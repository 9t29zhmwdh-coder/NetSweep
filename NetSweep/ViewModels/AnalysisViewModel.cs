using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NetSweep.Helpers;
using NetSweep.Models;
using NetSweep.Services;

namespace NetSweep.ViewModels;

public class AnalysisViewModel : ViewModelBase
{
    private readonly StorageConnection _connection;
    private readonly ScanService _scanService = new();
    private readonly DuplicateFinder _duplicateFinder = new();
    private readonly FileOperationService _fileOps = new();

    private ScanResult? _lastResult;
    private CancellationTokenSource? _cts;

    public string Title => Localization.Instance.Get("AnalysisTitleFormat", _connection.Name, _connection.Path);

    public ObservableCollection<FolderNode> TreeRoots { get; } = new();
    public ObservableCollection<FileEntry> Files { get; } = new();
    public ObservableCollection<DuplicateGroup> Duplicates { get; } = new();

    // ---- Filter inputs ----
    private string _olderThanDays = "";
    public string OlderThanDays { get => _olderThanDays; set => SetField(ref _olderThanDays, value); }

    private string _largerThan = "";
    public string LargerThan { get => _largerThan; set => SetField(ref _largerThan, value); }

    private string _extensionFilter = "";
    public string ExtensionFilter { get => _extensionFilter; set => SetField(ref _extensionFilter, value); }

    private string _nameFilter = "";
    public string NameFilter { get => _nameFilter; set => SetField(ref _nameFilter, value); }

    // ---- State ----
    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set { SetField(ref _isBusy, value); OnPropertyChanged(nameof(IsIdle)); RaiseAll(); } }
    public bool IsIdle => !_isBusy;

    private string _status = Localization.Instance.Get("ClickScanToStart");
    public string Status { get => _status; set => SetField(ref _status, value); }

    private string _progress = "";
    public string Progress { get => _progress; set => SetField(ref _progress, value); }

    private string _summary = "";
    public string Summary { get => _summary; set => SetField(ref _summary, value); }

    // ---- Commands ----
    public RelayCommand ScanCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand ApplyFilterCommand { get; }
    public RelayCommand ResetFilterCommand { get; }
    public RelayCommand QuickOldCommand { get; }
    public RelayCommand QuickLargeCommand { get; }
    public RelayCommand FindDuplicatesCommand { get; }
    public RelayCommand ShowEmptyFoldersCommand { get; }
    public RelayCommand DeleteEmptyFoldersCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand QuarantineCommand { get; }
    public RelayCommand CopyCommand { get; }
    public RelayCommand ExportFilesCommand { get; }
    public RelayCommand ExportDuplicatesCommand { get; }

    public AnalysisViewModel(StorageConnection connection)
    {
        _connection = connection;

        var progress = new Progress<string>(p => Progress = p);
        _scanService.Progress = progress;
        _duplicateFinder.Progress = progress;
        _fileOps.Progress = progress;

        ScanCommand = new RelayCommand(async _ => await ScanAsync(), _ => IsIdle);
        CancelCommand = new RelayCommand(_ => _cts?.Cancel(), _ => IsBusy);
        ApplyFilterCommand = new RelayCommand(_ => ApplyFilter(), _ => HasData());
        ResetFilterCommand = new RelayCommand(_ => ResetFilter(), _ => HasData());
        QuickOldCommand = new RelayCommand(_ => { OlderThanDays = "365"; ApplyFilter(); }, _ => HasData());
        QuickLargeCommand = new RelayCommand(_ => { LargerThan = "500 MB"; ApplyFilter(); }, _ => HasData());
        FindDuplicatesCommand = new RelayCommand(async _ => await FindDuplicatesAsync(), _ => HasData() && IsIdle);
        ShowEmptyFoldersCommand = new RelayCommand(_ => ShowEmptyFolders(), _ => HasData());
        DeleteEmptyFoldersCommand = new RelayCommand(async _ => await DeleteEmptyFoldersAsync(), _ => HasData() && IsIdle);
        DeleteCommand = new RelayCommand(async p => { try { await DeleteAsync(p as IList); } catch (Exception ex) { Status = ex.Message; } }, _ => IsIdle && _lastResult != null);
        QuarantineCommand = new RelayCommand(async p => { try { await QuarantineAsync(p as IList); } catch (Exception ex) { Status = ex.Message; } }, _ => IsIdle && _lastResult != null);
        CopyCommand = new RelayCommand(async p => { try { await CopyAsync(p as IList); } catch (Exception ex) { Status = ex.Message; } }, _ => IsIdle);
        ExportFilesCommand = new RelayCommand(_ => ExportFiles(), _ => Files.Count > 0);
        ExportDuplicatesCommand = new RelayCommand(_ => ExportDuplicates(), _ => Duplicates.Count > 0);

        Localization.Instance.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Title));
    }

    private bool HasData() => _lastResult != null;

    private async Task ScanAsync()
    {
        _cts = new CancellationTokenSource();
        IsBusy = true;
        Status = Localization.Instance.Get("Scanning");
        Duplicates.Clear();
        try
        {
            _lastResult = await _scanService.ScanAsync(_connection.Path, _cts.Token);
            TreeRoots.Clear();
            if (_lastResult.Tree != null) TreeRoots.Add(_lastResult.Tree);
            ApplyFilter();
            Summary = Localization.Instance.Get("ScanSummaryFormat",
                _lastResult.TotalFiles, _lastResult.TotalFolders,
                ByteSize.Format(_lastResult.TotalSize), _lastResult.EmptyFolders.Count);
            Status = _lastResult.Errors.Count > 0
                ? Localization.Instance.Get("ScanDoneWithWarnings", _lastResult.Errors.Count)
                : Localization.Instance.Get("ScanComplete");
        }
        catch (OperationCanceledException)
        {
            Status = Localization.Instance.Get("ScanCancelled");
        }
        catch (Exception ex)
        {
            Status = Localization.Instance.Get("ScanError");
            MessageBox.Show(ex.Message, Localization.Instance.Get("ScanErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
            Progress = "";
        }
    }

    private void ApplyFilter()
    {
        if (_lastResult == null) return;

        IEnumerable<FileEntry> query = _lastResult.Files;

        if (int.TryParse(OlderThanDays, out int days) && days > 0)
            query = query.Where(f => f.AgeDays >= days);

        long? minSize = ByteSize.Parse(LargerThan);
        if (minSize.HasValue)
            query = query.Where(f => f.Size >= minSize.Value);

        if (!string.IsNullOrWhiteSpace(ExtensionFilter))
        {
            var exts = ExtensionFilter.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.StartsWith('.') ? e.ToLowerInvariant() : "." + e.ToLowerInvariant())
                .ToHashSet();
            query = query.Where(f => exts.Contains(f.Extension));
        }

        if (!string.IsNullOrWhiteSpace(NameFilter))
            query = query.Where(f => f.Name.Contains(NameFilter, StringComparison.OrdinalIgnoreCase));

        var list = query.OrderByDescending(f => f.Size).ToList();
        Files.Clear();
        foreach (var f in list) Files.Add(f);

        long sum = list.Sum(f => f.Size);
        Status = Localization.Instance.Get("FilesFilteredFormat", list.Count, ByteSize.Format(sum));
        ExportFilesCommand.RaiseCanExecuteChanged();
    }

    private void ResetFilter()
    {
        OlderThanDays = LargerThan = ExtensionFilter = NameFilter = "";
        ApplyFilter();
    }

    private async Task FindDuplicatesAsync()
    {
        if (_lastResult == null) return;
        _cts = new CancellationTokenSource();
        IsBusy = true;
        Status = Localization.Instance.Get("SearchingDuplicates");
        try
        {
            var groups = await _duplicateFinder.FindAsync(_lastResult.Files, _cts.Token);
            Duplicates.Clear();
            foreach (var g in groups) Duplicates.Add(g);

            long reclaim = groups.Sum(g => g.ReclaimableBytes);
            Status = Localization.Instance.Get("DuplicateGroupsFormat", groups.Count, ByteSize.Format(reclaim));
        }
        catch (OperationCanceledException) { Status = Localization.Instance.Get("DuplicateSearchCancelled"); }
        finally { IsBusy = false; Progress = ""; ExportDuplicatesCommand.RaiseCanExecuteChanged(); }
    }

    private void ShowEmptyFolders()
    {
        if (_lastResult == null) return;
        if (_lastResult.EmptyFolders.Count == 0)
        {
            MessageBox.Show(Localization.Instance.Get("NoEmptyFoldersFound"), Localization.Instance.Get("EmptyFoldersTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        string list = string.Join('\n', _lastResult.EmptyFolders.Take(50));
        if (_lastResult.EmptyFolders.Count > 50) list += Localization.Instance.Get("MoreSuffixFormat", _lastResult.EmptyFolders.Count - 50);
        MessageBox.Show(list, Localization.Instance.Get("EmptyFoldersCountTitleFormat", _lastResult.EmptyFolders.Count),
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async Task DeleteEmptyFoldersAsync()
    {
        if (_lastResult == null || _lastResult.EmptyFolders.Count == 0)
        {
            MessageBox.Show(Localization.Instance.Get("NoEmptyFoldersToRemove"), Localization.Instance.Get("EmptyFoldersTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var answer = MessageBox.Show(
            Localization.Instance.Get("RemoveEmptyFoldersConfirmFormat", _lastResult.EmptyFolders.Count),
            Localization.Instance.Get("RemoveEmptyFoldersTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        IsBusy = true;
        try
        {
            var report = await _fileOps.DeleteEmptyFoldersAsync(_lastResult.EmptyFolders, _cts.Token);
            Status = Localization.Instance.Get("EmptyFoldersResultFormat", report.Summary);
        }
        finally { IsBusy = false; }
    }

    // ---- Destructive / move operations on the current selection ----

    private List<FileEntry>? GetSelection(IList? selected)
    {
        var files = selected?.OfType<FileEntry>().ToList() ?? new List<FileEntry>();
        if (files.Count == 0)
        {
            MessageBox.Show(Localization.Instance.Get("NoSelectionMessage"),
                Localization.Instance.Get("NoSelectionTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
            return null;
        }
        return files;
    }

    private async Task DeleteAsync(IList? selected)
    {
        var files = GetSelection(selected);
        if (files == null) return;

        long total = files.Sum(f => f.Size);

        // First warning
        var first = MessageBox.Show(
            Localization.Instance.Get("DeleteConfirmFormat", files.Count, ByteSize.Format(total)),
            Localization.Instance.Get("DeleteConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (first != MessageBoxResult.Yes) return;

        // Second warning: explicit point of no return
        var second = MessageBox.Show(
            Localization.Instance.Get("DeleteFinalWarning"),
            Localization.Instance.Get("FinalWarningTitle"), MessageBoxButton.YesNo, MessageBoxImage.Stop);
        if (second != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        IsBusy = true;
        try
        {
            var report = await _fileOps.DeletePermanentAsync(files, _cts.Token);
            // Drop files that no longer exist, then rebuild the filtered list.
            _lastResult!.Files.RemoveAll(f => files.Contains(f) && !File.Exists(f.FullPath));
            ApplyFilter();
            Status = Localization.Instance.Get("DeleteResultPrefix") + report.Summary;
            ShowErrors(report);
        }
        finally { IsBusy = false; Progress = ""; }
    }

    private async Task QuarantineAsync(IList? selected)
    {
        var files = GetSelection(selected);
        if (files == null) return;

        string target = _connection.QuarantineFolder;
        if (string.IsNullOrWhiteSpace(target))
        {
            var dlg = new OpenFolderDialog { Title = Localization.Instance.Get("ChooseQuarantineFolderPrompt") };
            if (dlg.ShowDialog() != true) return;
            target = dlg.FolderName;
        }

        var answer = MessageBox.Show(
            Localization.Instance.Get("MoveToQuarantineConfirmFormat", files.Count, target),
            Localization.Instance.Get("MoveToQuarantineTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;

        _cts = new CancellationTokenSource();
        IsBusy = true;
        try
        {
            var report = await _fileOps.MoveToQuarantineAsync(files, _connection.Path, target, _cts.Token);
            _lastResult!.Files.RemoveAll(f => files.Contains(f) && !File.Exists(f.FullPath));
            ApplyFilter();
            Status = Localization.Instance.Get("QuarantineResultPrefix") + report.Summary;
            ShowErrors(report);
        }
        finally { IsBusy = false; Progress = ""; }
    }

    private async Task CopyAsync(IList? selected)
    {
        var files = GetSelection(selected);
        if (files == null) return;

        var dlg = new OpenFolderDialog { Title = Localization.Instance.Get("ChooseCopyTargetTitle") };
        if (dlg.ShowDialog() != true) return;

        _cts = new CancellationTokenSource();
        IsBusy = true;
        try
        {
            var report = await _fileOps.CopyAsync(files, _connection.Path, dlg.FolderName, _cts.Token);
            Status = Localization.Instance.Get("CopyResultPrefix") + report.Summary;
            ShowErrors(report);
        }
        finally { IsBusy = false; Progress = ""; }
    }

    private void ExportFiles()
    {
        var dlg = new SaveFileDialog
        {
            Title = Localization.Instance.Get("ExportFilesDialogTitle"),
            Filter = Localization.Instance.Get("CsvFilterLabel"),
            FileName = $"NetSweep_Files_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (dlg.ShowDialog() == true)
        {
            ReportService.ExportFiles(Files, dlg.FileName);
            Status = Localization.Instance.Get("ExportedPrefix") + dlg.FileName;
        }
    }

    private void ExportDuplicates()
    {
        var dlg = new SaveFileDialog
        {
            Title = Localization.Instance.Get("ExportDuplicatesDialogTitle"),
            Filter = Localization.Instance.Get("CsvFilterLabel"),
            FileName = $"NetSweep_Duplicates_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (dlg.ShowDialog() == true)
        {
            ReportService.ExportDuplicates(Duplicates, dlg.FileName);
            Status = Localization.Instance.Get("ExportedPrefix") + dlg.FileName;
        }
    }

    private static void ShowErrors(OperationReport report)
    {
        if (report.Errors.Count == 0) return;
        string text = string.Join('\n', report.Errors.Take(20));
        if (report.Errors.Count > 20) text += Localization.Instance.Get("MoreSuffixFormat", report.Errors.Count - 20);
        MessageBox.Show(text, Localization.Instance.Get("ErrorsCountTitleFormat", report.Errors.Count), MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void RaiseAll()
    {
        ScanCommand.RaiseCanExecuteChanged();
        CancelCommand.RaiseCanExecuteChanged();
        ApplyFilterCommand.RaiseCanExecuteChanged();
        ResetFilterCommand.RaiseCanExecuteChanged();
        QuickOldCommand.RaiseCanExecuteChanged();
        QuickLargeCommand.RaiseCanExecuteChanged();
        FindDuplicatesCommand.RaiseCanExecuteChanged();
        ShowEmptyFoldersCommand.RaiseCanExecuteChanged();
        DeleteEmptyFoldersCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
        QuarantineCommand.RaiseCanExecuteChanged();
        CopyCommand.RaiseCanExecuteChanged();
    }
}
