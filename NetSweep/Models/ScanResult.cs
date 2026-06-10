namespace NetSweep.Models;

/// <summary>Aggregated outcome of a scan over a connection's path.</summary>
public class ScanResult
{
    public string RootPath { get; set; } = string.Empty;
    public FolderNode? Tree { get; set; }
    public List<FileEntry> Files { get; set; } = new();
    public List<string> EmptyFolders { get; set; } = new();
    public long TotalSize { get; set; }
    public int TotalFiles { get; set; }
    public int TotalFolders { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>A set of files that are byte-for-byte identical (same hash + size).</summary>
public class DuplicateGroup
{
    public string Hash { get; set; } = string.Empty;
    public long Size { get; set; }
    public List<FileEntry> Files { get; set; } = new();

    public string SizeText => Helpers.ByteSize.Format(Size);

    /// <summary>Space that could be reclaimed by keeping a single copy.</summary>
    public long ReclaimableBytes => Size * Math.Max(0, Files.Count - 1);
    public string ReclaimableText => Helpers.ByteSize.Format(ReclaimableBytes);
}
