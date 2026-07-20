using NetSweep.Services;
using NetSweep.Models;

namespace NetSweep.Tests.Services;

/// <summary>
/// Tests for FileOperationService: delete, move to quarantine, copy, and empty folder cleanup operations.
/// </summary>
public class FileOperationServiceTests : IAsyncLifetime
{
    private string _testDirectory = null!;
    private string _quarantineDirectory = null!;
    private FileOperationService _service = null!;

    public Task InitializeAsync()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileOps_Test_{Guid.NewGuid()}");
        _quarantineDirectory = Path.Combine(_testDirectory, "quarantine");
        Directory.CreateDirectory(_testDirectory);
        _service = new FileOperationService();
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
    public async Task DeletePermanentAsync_WithValidFiles_DeletesFilesAndReportsSuccess()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "test.txt");
        File.WriteAllText(filePath, "test content");
        var file = new FileEntry { FullPath = filePath, Name = "test.txt", Size = 12 };

        // Act
        var report = await _service.DeletePermanentAsync(new[] { file }, CancellationToken.None);

        // Assert
        Assert.False(File.Exists(filePath));
        Assert.Equal(1, report.Succeeded);
        Assert.Equal(0, report.Failed);
        Assert.Equal(12L, report.BytesFreed);
    }

    [Fact]
    public async Task DeletePermanentAsync_WithMultipleFiles_DeletesAllAndAccumulatesStats()
    {
        // Arrange
        var file1Path = Path.Combine(_testDirectory, "file1.txt");
        var file2Path = Path.Combine(_testDirectory, "file2.txt");
        File.WriteAllText(file1Path, "content1");
        File.WriteAllText(file2Path, "content2_longer");
        var file1 = new FileEntry { FullPath = file1Path, Name = "file1.txt", Size = 8 };
        var file2 = new FileEntry { FullPath = file2Path, Name = "file2.txt", Size = 15 };

        // Act
        var report = await _service.DeletePermanentAsync(new[] { file1, file2 }, CancellationToken.None);

        // Assert
        Assert.False(File.Exists(file1Path));
        Assert.False(File.Exists(file2Path));
        Assert.Equal(2, report.Succeeded);
        Assert.Equal(0, report.Failed);
        Assert.Equal(23L, report.BytesFreed);
    }

    [Fact]
    public async Task DeletePermanentAsync_WithNonExistentFile_RecordsFailure()
    {
        // Arrange
        var file = new FileEntry { FullPath = "/nonexistent/path/file.txt", Name = "file.txt", Size = 100 };

        // Act
        var report = await _service.DeletePermanentAsync(new[] { file }, CancellationToken.None);

        // Assert
        Assert.Equal(0, report.Succeeded);
        Assert.Equal(1, report.Failed);
        Assert.True(report.Errors.Count > 0);
    }

    [Fact]
    public async Task MoveToQuarantineAsync_WithValidFile_MovesFilePreservingStructure()
    {
        // Arrange
        var subDir = Path.Combine(_testDirectory, "subfolder");
        Directory.CreateDirectory(subDir);
        var filePath = Path.Combine(subDir, "test.txt");
        File.WriteAllText(filePath, "test content");
        var file = new FileEntry { FullPath = filePath, Name = "test.txt", Size = 12 };

        // Act
        var report = await _service.MoveToQuarantineAsync(
            new[] { file }, _testDirectory, _quarantineDirectory, CancellationToken.None);

        // Assert
        Assert.False(File.Exists(filePath));
        var quarantinedPath = Path.Combine(_quarantineDirectory, "subfolder", "test.txt");
        Assert.True(File.Exists(quarantinedPath));
        Assert.Equal(1, report.Succeeded);
        Assert.Equal(0, report.Failed);
    }

    [Fact]
    public async Task MoveToQuarantineAsync_WithMultipleFiles_MovesAllFilesAndCreatesNestedDirectories()
    {
        // Arrange
        var sub1 = Path.Combine(_testDirectory, "folder1");
        var sub2 = Path.Combine(_testDirectory, "folder2");
        Directory.CreateDirectory(sub1);
        Directory.CreateDirectory(sub2);

        var file1Path = Path.Combine(sub1, "file1.txt");
        var file2Path = Path.Combine(sub2, "file2.txt");
        File.WriteAllText(file1Path, "content1");
        File.WriteAllText(file2Path, "content2");

        var files = new[]
        {
            new FileEntry { FullPath = file1Path, Name = "file1.txt", Size = 8 },
            new FileEntry { FullPath = file2Path, Name = "file2.txt", Size = 8 }
        };

        // Act
        var report = await _service.MoveToQuarantineAsync(
            files, _testDirectory, _quarantineDirectory, CancellationToken.None);

        // Assert
        Assert.Equal(2, report.Succeeded);
        Assert.Equal(0, report.Failed);
        Assert.True(File.Exists(Path.Combine(_quarantineDirectory, "folder1", "file1.txt")));
        Assert.True(File.Exists(Path.Combine(_quarantineDirectory, "folder2", "file2.txt")));
    }

    [Fact]
    public async Task CopyAsync_WithValidFile_CopiesFileToTargetDirectory()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "source.txt");
        File.WriteAllText(filePath, "test content");
        var targetDir = Path.Combine(_testDirectory, "target");
        var file = new FileEntry { FullPath = filePath, Name = "source.txt", Size = 12 };

        // Act
        var report = await _service.CopyAsync(new[] { file }, _testDirectory, targetDir, CancellationToken.None);

        // Assert
        Assert.True(File.Exists(filePath)); // Original still exists
        var targetPath = Path.Combine(targetDir, "source.txt");
        Assert.True(File.Exists(targetPath));
        Assert.Equal(1, report.Succeeded);
        Assert.Equal(0, report.Failed);
    }

    [Fact]
    public async Task CopyAsync_WithMultipleFilesInSubdirectories_CopiesAllPreservingStructure()
    {
        // Arrange
        var sub = Path.Combine(_testDirectory, "subfolder");
        Directory.CreateDirectory(sub);
        var filePath = Path.Combine(sub, "test.txt");
        File.WriteAllText(filePath, "test content");
        var targetDir = Path.Combine(_testDirectory, "backup");

        var file = new FileEntry { FullPath = filePath, Name = "test.txt", Size = 12 };

        // Act
        var report = await _service.CopyAsync(new[] { file }, _testDirectory, targetDir, CancellationToken.None);

        // Assert
        var targetPath = Path.Combine(targetDir, "subfolder", "test.txt");
        Assert.True(File.Exists(targetPath));
        Assert.Equal(1, report.Succeeded);
    }

    [Fact]
    public async Task DeleteEmptyFoldersAsync_WithEmptyFolders_DeletesFoldersAndReportsSuccess()
    {
        // Arrange
        var emptyDir1 = Path.Combine(_testDirectory, "empty1");
        var emptyDir2 = Path.Combine(_testDirectory, "empty2");
        Directory.CreateDirectory(emptyDir1);
        Directory.CreateDirectory(emptyDir2);

        // Act
        var report = await _service.DeleteEmptyFoldersAsync(
            new[] { emptyDir1, emptyDir2 }, CancellationToken.None);

        // Assert
        Assert.False(Directory.Exists(emptyDir1));
        Assert.False(Directory.Exists(emptyDir2));
        Assert.Equal(2, report.Succeeded);
        Assert.Equal(0, report.Failed);
    }

    [Fact]
    public async Task DeleteEmptyFoldersAsync_WithNestedEmptyFolders_DeletesDeepestFirst()
    {
        // Arrange
        var nestedDir = Path.Combine(_testDirectory, "level1", "level2", "level3");
        Directory.CreateDirectory(nestedDir);

        // Act
        var report = await _service.DeleteEmptyFoldersAsync(
            new[] { nestedDir }, CancellationToken.None);

        // Assert
        Assert.False(Directory.Exists(nestedDir));
        Assert.Equal(1, report.Succeeded);
    }

    [Fact]
    public async Task DeleteEmptyFoldersAsync_WithNonEmptyFolder_SkipsFolder()
    {
        // Arrange
        var folderWithFile = Path.Combine(_testDirectory, "nonempty");
        Directory.CreateDirectory(folderWithFile);
        File.WriteAllText(Path.Combine(folderWithFile, "file.txt"), "content");

        // Act
        var report = await _service.DeleteEmptyFoldersAsync(
            new[] { folderWithFile }, CancellationToken.None);

        // Assert
        Assert.True(Directory.Exists(folderWithFile)); // Should not be deleted
        Assert.Equal(0, report.Succeeded);
    }

    [Fact]
    public async Task DeleteEmptyFoldersAsync_WithNonExistentFolder_RecordsNoError()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act
        var report = await _service.DeleteEmptyFoldersAsync(
            new[] { nonExistentDir }, CancellationToken.None);

        // Assert
        Assert.Equal(0, report.Succeeded);
        Assert.Equal(0, report.Failed); // Non-existent folder is just skipped silently
    }

    [Fact]
    public async Task DeletePermanentAsync_WithCancellationToken_HonoursCancellation()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "test.txt");
        File.WriteAllText(filePath, "content");
        var file = new FileEntry { FullPath = filePath, Name = "test.txt", Size = 7 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.DeletePermanentAsync(new[] { file }, cts.Token)
        );
    }
}
