namespace MediaService.Repositories;

public record FileData(
    Stream Content,
    string ContentType,
    string FileName
);
