using System.Collections.ObjectModel;
using System.Windows;
using NetSweep.Helpers;
using NetSweep.Models;
using NetSweep.Services;
using NetSweep.Views;

namespace NetSweep.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ConnectionStore _store = new();
    private readonly NetworkConnectionService _network = new();

    public ObservableCollection<StorageConnection> Connections { get; } = new();

    private StorageConnection? _selected;
    public StorageConnection? Selected
    {
        get => _selected;
        set { SetField(ref _selected, value); RaiseCommandStates(); }
    }

    private string _status = "Bereit.";
    public string Status { get => _status; set => SetField(ref _status, value); }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand RemoveCommand { get; }
    public RelayCommand ConnectCommand { get; }
    public RelayCommand OpenCommand { get; }

    public MainViewModel()
    {
        AddCommand = new RelayCommand(_ => Add());
        EditCommand = new RelayCommand(_ => Edit(), _ => Selected != null);
        RemoveCommand = new RelayCommand(_ => Remove(), _ => Selected != null);
        ConnectCommand = new RelayCommand(_ => Connect(), _ => Selected != null);
        OpenCommand = new RelayCommand(_ => Open(), _ => Selected != null);

        foreach (var c in _store.Load()) Connections.Add(c);
    }

    private void Add()
    {
        var connection = new StorageConnection { Name = "Neue Verbindung" };
        var dialog = new ConnectionEditDialog(connection) { Owner = Application.Current.MainWindow };
        if (dialog.ShowDialog() == true)
        {
            Connections.Add(connection);
            Persist();
            Selected = connection;
            Status = "Verbindung hinzugefuegt.";
        }
    }

    private void Edit()
    {
        if (Selected == null) return;
        var dialog = new ConnectionEditDialog(Selected) { Owner = Application.Current.MainWindow };
        if (dialog.ShowDialog() == true)
        {
            Persist();
            Status = "Verbindung gespeichert.";
            RefreshList();
        }
    }

    private void Remove()
    {
        if (Selected == null) return;
        var answer = MessageBox.Show(
            $"Verbindung „{Selected.Name}“ wirklich entfernen?\n\n(Es werden keine Dateien geloescht, nur dieser Eintrag.)",
            "Verbindung entfernen", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer == MessageBoxResult.Yes)
        {
            _network.Disconnect(Selected);
            Connections.Remove(Selected);
            Persist();
            Status = "Verbindung entfernt.";
        }
    }

    private void Connect()
    {
        if (Selected == null) return;
        Status = "Verbinde...";
        var (ok, message) = _network.Connect(Selected);
        RefreshList();
        Status = message;
        if (!ok)
            MessageBox.Show(message, "Verbindung", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void Open()
    {
        if (Selected == null) return;

        if (!Selected.IsConnected)
        {
            var (ok, message) = _network.Connect(Selected);
            RefreshList();
            if (!ok)
            {
                MessageBox.Show(message, "Verbindung", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        var window = new AnalysisWindow(Selected) { Owner = Application.Current.MainWindow };
        window.Show();
    }

    private void Persist() => _store.Save(Connections);

    private void RefreshList()
    {
        // Force list refresh of computed columns (IsConnected).
        var snapshot = Connections.ToList();
        var current = Selected;
        Connections.Clear();
        foreach (var c in snapshot) Connections.Add(c);
        Selected = current;
    }

    private void RaiseCommandStates()
    {
        EditCommand.RaiseCanExecuteChanged();
        RemoveCommand.RaiseCanExecuteChanged();
        ConnectCommand.RaiseCanExecuteChanged();
        OpenCommand.RaiseCanExecuteChanged();
    }
}
