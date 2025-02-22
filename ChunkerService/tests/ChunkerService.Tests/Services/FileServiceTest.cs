using ChunkerService.Services;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Services;

[TestSubject(typeof(FileService))]
public class FileServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        _testDirectory = Path.Combine("tmp", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _fileService = new FileService();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void DirectoryExists_ReturnsTrue_WhenDirectoryExists()
    {
        // Act
        var exists = _fileService.DirectoryExists(_testDirectory);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public void DirectoryExists_ReturnsFalse_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

        // Act
        var exists = _fileService.DirectoryExists(nonExistentDir);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public void CreateDirectory_CreatesDirectory()
    {
        // Arrange
        var newDir = Path.Combine(_testDirectory, "newDir");

        // Act
        _fileService.CreateDirectory(newDir);

        // Assert
        Assert.True(Directory.Exists(newDir));
    }

    [Fact]
    public void DeleteDirectory_DeletesDirectory()
    {
        // Arrange
        var dirToDelete = Path.Combine(_testDirectory, "dirToDelete");
        Directory.CreateDirectory(dirToDelete);

        // Act
        _fileService.DeleteDirectory(dirToDelete, recursive: false);

        // Assert
        Assert.False(Directory.Exists(dirToDelete));
    }

    [Fact]
    public async Task GetFiles_ReturnsFilesInDirectory()
    {
        // Arrange
        var filePath1 = Path.Combine(_testDirectory, "file1.txt");
        var filePath2 = Path.Combine(_testDirectory, "file2.txt");

        await _fileService.WriteAllTextAsync(filePath1, "Content 1", CancellationToken.None);
        await _fileService.WriteAllTextAsync(filePath2, "Content 2", CancellationToken.None);

        // Act
        var files = _fileService.GetFiles(_testDirectory);

        // Assert
        Assert.Contains(filePath1, files);
        Assert.Contains(filePath2, files);
        Assert.Equal(2, files.Length);
    }

    [Fact]
    public void CreateFile_CreatesFile()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "testfile.txt");

        // Act
        using (var _ = _fileService.CreateFile(filePath)) { }

        // Assert
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void OpenReadFile_ReadsFileContent()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "testfile.txt");
        const string expectedContent = "Hello, world!";
        File.WriteAllText(filePath, expectedContent);

        // Act
        using var stream = _fileService.OpenReadFile(filePath);
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        // Assert
        Assert.Equal(expectedContent, content);
    }

    [Fact]
    public async Task WriteAllTextAsync_WritesContentToFile()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "asyncTestFile.txt");
        const string expectedContent = "Async content";

        // Act
        await _fileService.WriteAllTextAsync(filePath, expectedContent, CancellationToken.None);

        // Assert
        var actualContent = await File.ReadAllTextAsync(filePath);
        Assert.Equal(expectedContent, actualContent);
    }
}
