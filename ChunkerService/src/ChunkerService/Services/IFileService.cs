namespace ChunkerService.Services;

public interface IFileService
{
    bool DirectoryExists(string path);
    void CreateDirectory(string path);

    void DeleteDirectory(
        string path,
        bool recursive
    );

    string[] GetFiles(string path);

    Stream CreateFile(string path);
    Stream OpenReadFile(string path);

    Task WriteAllTextAsync(
        string path,
        string contents,
        CancellationToken cancellationToken
    );
}
