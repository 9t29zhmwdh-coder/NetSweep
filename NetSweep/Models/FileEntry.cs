namespace NetSweep.Models;

/// <summary>A single file discovered during a scan.</summary>
public class FileEntry
{
    public string FullPath { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public DateTime LastAccessed { get; set; }

    /// <summary>Optional content hash (only computed for duplicate detection).</summary>
    public string? Hash { get; set; }

    /// <summary>Human readable size, e.g. "1.4 GB".</summary>
    public string SizeText => Helpers.ByteSize.Format(Size);

    /// <summary>Age in days based on last modification.</summary>
    public int AgeDays => (int)(DateTime.Now - LastModified).TotalDays;
}
