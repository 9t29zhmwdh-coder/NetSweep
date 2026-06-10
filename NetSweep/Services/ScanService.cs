using System.IO;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>
/// Walks a directory tree and produces a ScanResult: an aggregated folder tree
/// (TreeSize-style), the full file list and the list of empty folders.
/// Runs off the UI thread; reports progress and honours cancellation.
/// </summary>
public class ScanService
{
    public IProgress<string>? Progress { get; set; }

    public Task<ScanResult> ScanAsync(string rootPath, CancellationToken token)
        => Task.Run(() => Scan(rootPath, token), token);

    private ScanResult Scan(string rootPath, CancellationToken token)
    {
        var result = new ScanResult { RootPath = rootPath };
        if (!Directory.Exists(rootPath))
        {
            result.Errors.Add($"Pfad nicht gefunden: {rootPath}");
            return result;
        }

        var root = BuildNode(new DirectoryInfo(rootPath), result, token);
        result.Tree = root;
        result.TotalSize = root?.Size ?? 0;
        AssignPercentages(root);
        return result;
    }

    private FolderNode? BuildNode(DirectoryInfo dir, ScanResult result, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var node = new FolderNode { Name = dir.Name, FullPath = dir.FullName };
        long size = 0;
        int fileCount = 0;

        // Files in this folder
        FileInfo[] files = Array.Empty<FileInfo>();
        try { files = dir.GetFiles(); }
        catch (Exception ex) { result.Errors.Add($"{dir.FullName}: {ex.Message}"); }

        foreach (var f in files)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var entry = new FileEntry
                {
                    FullPath = f.FullName,
                    Name = f.Name,
                    Extension = f.Extension.ToLowerInvariant(),
                    Size = f.Length,
                    LastModified = f.LastWriteTime,
                    LastAccessed = f.LastAccessTime
                };
                result.Files.Add(entry);
                size += f.Length;
                fileCount++;
            }
            catch (Exception ex) { result.Errors.Add($"{f.FullName}: {ex.Message}"); }
        }

        // Sub-folders
        DirectoryInfo[] subDirs = Array.Empty<DirectoryInfo>();
        try { subDirs = dir.GetDirectories(); }
        catch (Exception ex) { result.Errors.Add($"{dir.FullName}: {ex.Message}"); }

        foreach (var sub in subDirs)
        {
            // Skip reparse points (symlinks/junctions) to avoid loops.
            if (sub.Attributes.HasFlag(FileAttributes.ReparsePoint)) continue;

            Progress?.Report(sub.FullName);
            var child = BuildNode(sub, result, token);
            if (child != null)
            {
                node.Children.Add(child);
                size += child.Size;
                fileCount += child.FileCount;
                node.FolderCount += 1 + child.FolderCount;
            }
        }

        node.Size = size;
        node.FileCount = fileCount;
        result.TotalFolders += 1;
        result.TotalFiles += files.Length;

        if (fileCount == 0 && node.Children.Count == 0)
            result.EmptyFolders.Add(dir.FullName);

        // Largest folders first for nicer display
        var ordered = node.Children.OrderByDescending(c => c.Size).ToList();
        node.Children.Clear();
        foreach (var c in ordered) node.Children.Add(c);

        return node;
    }

    private static void AssignPercentages(FolderNode? node)
    {
        if (node == null) return;
        foreach (var child in node.Children)
        {
            child.Percent = node.Size > 0 ? (double)child.Size / node.Size : 0;
            AssignPercentages(child);
        }
    }
}
