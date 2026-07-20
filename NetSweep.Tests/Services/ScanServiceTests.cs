using NetSweep.Services;
using NetSweep.Models;

namespace NetSweep.Tests.Services;

/// <summary>
/// Tests for ScanService: directory tree scanning, file discovery, and empty folder detection.
/// </summary>
public class ScanServiceTests : IAsyncLifetime
{
    private string _testDirectory = null!;
    private ScanService _service = null!;

    public Task InitializeAsync()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"NetSweep_Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _service = new ScanService();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ScanAsync_WithValidPath_ReturnsScanResult()
    {
        // Arrange
        var file = Path.Combine(_testDirectory, "test.txt");
        File.WriteAllText(file, "test content");

        // Act
        var result = await _service.ScanAsync(_testDirectory, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testDirectory, result.RootPath);
        Assert.NotNull(result.Tree);
        Assert.True(result.TotalFiles > 0);
        Assert.True(result.TotalSize > 0);
    }

    [Fact]
    public async Task ScanAsync_WithNonExistentPath_ReturnsErrorInResult()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testDirectory, "nonexistent");

        // Act
        var result = await _service.ScanAsync(nonExistentPath, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public async Task ScanAsync_WithEmptyDirectory_ReturnsZeroFiles()
    {
        // Arrange
        var emptyDir = Path.Combine(_testDirectory, "empty");
        Directory.CreateDirectory(emptyDir);

        // Act
        var result = await _service.ScanAsync(emptyDir, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalFiles);
        Assert.Equal(0, result.TotalSize);
    }

    [Fact]
    public async Task ScanAsync_WithMultipleFiles_CountsFilesAndCalculatesTotalSize()
    {
        // Arrange
        var file1 = Path.Combine(_testDirectory, "file1.txt");
        var file2 = Path.Combine(_testDirectory, "file2.txt");
        var content1 = "Hello";
        var content2 = "World!";
        File.WriteAllText(file1, content1);
        File.WriteAllText(file2, content2);

        // Act
        var result = await _service.ScanAsync(_testDirectory, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalFiles);
        Assert.Equal(content1.Length + content2.Length, result.TotalSize);
    }

    [Fact]
    public async Task ScanAsync_WithNestedDirectories_BuildsTreeStructure()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "subfolder");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "nested.txt"), "nested content");

        // Act
        var result = await _service.ScanAsync(_testDirectory, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Tree);
        Assert.True(result.Tree.Children.Count > 0);
        Assert.True(result.TotalFolders >= 1);
    }

    [Fact]
    public async Task ScanAsync_WithEmptySubfolders_IdentifiesEmptyFolders()
    {
        // Arrange
        var emptySubDir = Path.Combine(_testDirectory, "empty_sub");
        Directory.CreateDirectory(emptySubDir);
        var fileInRoot = Path.Combine(_testDirectory, "file.txt");
        File.WriteAllText(fileInRoot, "content");

        // Act
        var result = await _service.ScanAsync(_testDirectory, CancellationToken.None);

        // Assert
        Assert.True(result.EmptyFolders.Count > 0);
        Assert.Contains(emptySubDir, result.EmptyFolders);
    }

    [Fact]
    public async Task ScanAsync_WithCancellationToken_HonoursCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var file = Path.Combine(_testDirectory, "test.txt");
        File.WriteAllText(file, "content");
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.ScanAsync(_testDirectory, cts.Token)
        );
    }

    [Fact]
    public async Task ScanAsync_WithMultipleFilesInSubfolders_AggregatesSizeCorrectly()
    {
        // Arrange
        var subDir1 = Path.Combine(_testDirectory, "sub1");
        var subDir2 = Path.Combine(_testDirectory, "sub2");
        Directory.CreateDirectory(subDir1);
        Directory.CreateDirectory(subDir2);

        File.WriteAllText(Path.Combine(subDir1, "file1.txt"), "1000 bytes");
        File.WriteAllText(Path.Combine(subDir2, "file2.txt"), "2000 bytes");
        File.WriteAllText(Path.Combine(_testDirectory, "root.txt"), "3000 bytes");

        // Act
        var result = await _service.ScanAsync(_testDirectory, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.TotalFiles);
        Assert.True(result.TotalSize > 0);
        Assert.NotNull(result.Tree);
        Assert.True(result.Tree.Size > 0);
    }
}
