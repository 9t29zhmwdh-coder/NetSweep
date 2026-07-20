using NetSweep.Services;
using NetSweep.Models;

namespace NetSweep.Tests.Services;

/// <summary>
/// Tests for DuplicateFinder: SHA-256 based duplicate detection and classification.
/// </summary>
public class DuplicateFinderTests
{
    [Fact]
    public async Task FindAsync_WithDuplicateFiles_IdentifiesDuplicates()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var content = "identical content";
        var file1 = new FileEntry { FullPath = "file1.txt", Name = "file1.txt", Size = content.Length };
        var file2 = new FileEntry { FullPath = "file2.txt", Name = "file2.txt", Size = content.Length };
        var files = new[] { file1, file2 };

        // Create temporary test files
        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var path1 = Path.Combine(tempDir, "file1.txt");
            var path2 = Path.Combine(tempDir, "file2.txt");
            File.WriteAllText(path1, content);
            File.WriteAllText(path2, content);

            file1.FullPath = path1;
            file2.FullPath = path2;

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result.Count > 0);
            var dupGroup = result.First();
            Assert.Equal(2, dupGroup.Files.Count);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FindAsync_WithNoDuplicates_ReturnsEmptyList()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var file1 = new FileEntry { FullPath = "file1.txt", Name = "file1.txt", Size = 10 };
        var file2 = new FileEntry { FullPath = "file2.txt", Name = "file2.txt", Size = 20 };
        var files = new[] { file1, file2 };

        // Create temporary test files with different content
        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var path1 = Path.Combine(tempDir, "file1.txt");
            var path2 = Path.Combine(tempDir, "file2.txt");
            File.WriteAllText(path1, "content1");
            File.WriteAllText(path2, "content2_different");

            file1.FullPath = path1;
            file2.FullPath = path2;

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FindAsync_WithZeroByteFiles_IgnoresZeroByteFiles()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var file1 = new FileEntry { FullPath = "file1.txt", Name = "file1.txt", Size = 0 };
        var file2 = new FileEntry { FullPath = "file2.txt", Name = "file2.txt", Size = 0 };
        var files = new[] { file1, file2 };

        // Create temporary test files
        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var path1 = Path.Combine(tempDir, "file1.txt");
            var path2 = Path.Combine(tempDir, "file2.txt");
            File.WriteAllText(path1, "");
            File.WriteAllText(path2, "");

            file1.FullPath = path1;
            file2.FullPath = path2;

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FindAsync_WithMultipleDuplicateGroups_IdentifiesAllGroups()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var files = new List<FileEntry>();

        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            // Group 1: two identical files
            var path1a = Path.Combine(tempDir, "file1a.txt");
            var path1b = Path.Combine(tempDir, "file1b.txt");
            File.WriteAllText(path1a, "group1");
            File.WriteAllText(path1b, "group1");

            // Group 2: three identical files
            var path2a = Path.Combine(tempDir, "file2a.txt");
            var path2b = Path.Combine(tempDir, "file2b.txt");
            var path2c = Path.Combine(tempDir, "file2c.txt");
            File.WriteAllText(path2a, "group2_content");
            File.WriteAllText(path2b, "group2_content");
            File.WriteAllText(path2c, "group2_content");

            files.Add(new FileEntry { FullPath = path1a, Name = "file1a.txt", Size = 6 });
            files.Add(new FileEntry { FullPath = path1b, Name = "file1b.txt", Size = 6 });
            files.Add(new FileEntry { FullPath = path2a, Name = "file2a.txt", Size = 16 });
            files.Add(new FileEntry { FullPath = path2b, Name = "file2b.txt", Size = 16 });
            files.Add(new FileEntry { FullPath = path2c, Name = "file2c.txt", Size = 16 });

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.True(result.Count >= 2);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FindAsync_WithSingleFileOfEachSize_ReturnsEmpty()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var file = new FileEntry { FullPath = "file.txt", Name = "file.txt", Size = 10 };
        var files = new[] { file };

        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var path = Path.Combine(tempDir, "file.txt");
            File.WriteAllText(path, "unique_file");
            file.FullPath = path;

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task FindAsync_DuplicateGroup_CalculatesReclaimableBytesCorrectly()
    {
        // Arrange
        var finder = new DuplicateFinder();
        var fileSize = 100L;
        var file1 = new FileEntry { FullPath = "file1.txt", Name = "file1.txt", Size = fileSize };
        var file2 = new FileEntry { FullPath = "file2.txt", Name = "file2.txt", Size = fileSize };
        var file3 = new FileEntry { FullPath = "file3.txt", Name = "file3.txt", Size = fileSize };
        var files = new[] { file1, file2, file3 };

        var tempDir = Path.Combine(Path.GetTempPath(), $"DupTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var content = "x".PadRight(100);
            var path1 = Path.Combine(tempDir, "file1.txt");
            var path2 = Path.Combine(tempDir, "file2.txt");
            var path3 = Path.Combine(tempDir, "file3.txt");
            File.WriteAllText(path1, content);
            File.WriteAllText(path2, content);
            File.WriteAllText(path3, content);

            file1.FullPath = path1;
            file2.FullPath = path2;
            file3.FullPath = path3;

            // Act
            var result = await finder.FindAsync(files, CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            var dupGroup = result.First();
            // 3 files of size 100 each, keep 1, so reclaimable = 100 * (3 - 1) = 200
            Assert.Equal(200L, dupGroup.ReclaimableBytes);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
