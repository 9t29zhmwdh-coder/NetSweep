using System.Collections.ObjectModel;

namespace NetSweep.Models;

/// <summary>
/// A node in the TreeSize-style folder tree. Size is the aggregated size of all
/// files in this folder and its sub-folders.
/// </summary>
public class FolderNode
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public long Size { get; set; }
    public int FileCount { get; set; }
    public int FolderCount { get; set; }

    public ObservableCollection<FolderNode> Children { get; } = new();

    public string SizeText => Helpers.ByteSize.Format(Size);

    /// <summary>Share of the parent folder size, 0..1 (set during build).</summary>
    public double Percent { get; set; }

    public string PercentText => $"{Percent * 100:0.0} %";
}
