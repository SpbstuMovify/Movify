namespace ChunkerService.Services;

public class FileService : IFileService
{
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void DeleteDirectory(
        string path,
        bool recursive
    ) =>
        Directory.Delete(path, recursive);

    public string[] GetFiles(string path) => Directory.GetFiles(path);

    public Stream CreateFile(string path) => File.Create(path);
    public Stream OpenReadFile(string path) => File.OpenRead(path);

    public Task WriteAllTextAsync(
        string path,
        string contents,
        CancellationToken cancellationToken
    ) =>
        File.WriteAllTextAsync(path, contents, cancellationToken);
}
