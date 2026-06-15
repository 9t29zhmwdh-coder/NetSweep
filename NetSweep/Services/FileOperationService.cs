using System.IO;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>Outcome of a batch file operation.</summary>
public class OperationReport
{
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public long BytesFreed { get; set; }
    public List<string> Errors { get; } = new();
    public string Summary =>
        $"{Succeeded} erfolgreich, {Failed} fehlgeschlagen, {Helpers.ByteSize.Format(BytesFreed)} freigegeben.";
}

/// <summary>
/// Deletes, moves or copies files. Delete is PERMANENT by design (matches the
/// double-confirmation flow in the UI). MoveToQuarantine is the safer option:
/// files are relocated into a quarantine folder, preserving their sub-path.
/// </summary>
public class FileOperationService
{
    public IProgress<string>? Progress { get; set; }

    public Task<OperationReport> DeletePermanentAsync(IEnumerable<FileEntry> files, CancellationToken token)
        => Task.Run(() =>
        {
            var report = new OperationReport();
            foreach (var f in files)
            {
                token.ThrowIfCancellationRequested();
                Progress?.Report(f.Name);
                try
                {
                    File.SetAttributes(f.FullPath, FileAttributes.Normal);
                    File.Delete(f.FullPath);
                    report.Succeeded++;
                    report.BytesFreed += f.Size;
                }
                catch (Exception ex)
                {
                    report.Failed++;
                    report.Errors.Add($"{f.FullPath}: {ex.Message}");
                }
            }
            return report;
        }, token);

    public Task<OperationReport> MoveToQuarantineAsync(
        IEnumerable<FileEntry> files, string rootPath, string quarantineFolder, CancellationToken token)
        => Task.Run(() =>
        {
            var report = new OperationReport();
            Directory.CreateDirectory(quarantineFolder);
            foreach (var f in files)
            {
                token.ThrowIfCancellationRequested();
                Progress?.Report(f.Name);
                try
                {
                    string target = BuildTargetPath(f.FullPath, rootPath, quarantineFolder);
                    var dir = Path.GetDirectoryName(target);
                    if (dir != null) Directory.CreateDirectory(dir);
                    File.Move(f.FullPath, target, overwrite: false);
                    report.Succeeded++;
                    report.BytesFreed += f.Size;
                }
                catch (Exception ex)
                {
                    report.Failed++;
                    report.Errors.Add($"{f.FullPath}: {ex.Message}");
                }
            }
            return report;
        }, token);

    public Task<OperationReport> CopyAsync(
        IEnumerable<FileEntry> files, string rootPath, string targetFolder, CancellationToken token)
        => Task.Run(() =>
        {
            var report = new OperationReport();
            Directory.CreateDirectory(targetFolder);
            foreach (var f in files)
            {
                token.ThrowIfCancellationRequested();
                Progress?.Report(f.Name);
                try
                {
                    string target = BuildTargetPath(f.FullPath, rootPath, targetFolder);
                    var dir = Path.GetDirectoryName(target);
                    if (dir != null) Directory.CreateDirectory(dir);
                    File.Copy(f.FullPath, target, overwrite: true);
                    report.Succeeded++;
                    report.BytesFreed += f.Size;
                }
                catch (Exception ex)
                {
                    report.Failed++;
                    report.Errors.Add($"{f.FullPath}: {ex.Message}");
                }
            }
            return report;
        }, token);

    public Task<OperationReport> DeleteEmptyFoldersAsync(IEnumerable<string> folders, CancellationToken token)
        => Task.Run(() =>
        {
            var report = new OperationReport();
            // Deepest first so parents become empty too.
            foreach (var folder in folders.OrderByDescending(p => p.Length))
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    if (Directory.Exists(folder) &&
                        !Directory.EnumerateFileSystemEntries(folder).Any())
                    {
                        Directory.Delete(folder);
                        report.Succeeded++;
                    }
                }
                catch (Exception ex)
                {
                    report.Failed++;
                    report.Errors.Add($"{folder}: {ex.Message}");
                }
            }
            return report;
        }, token);

    private static string BuildTargetPath(string sourceFile, string rootPath, string targetRoot)
    {
        string relative = Path.GetRelativePath(rootPath, sourceFile);
        if (relative.StartsWith("..")) relative = Path.GetFileName(sourceFile);
        return Path.Combine(targetRoot, relative);
    }
}
