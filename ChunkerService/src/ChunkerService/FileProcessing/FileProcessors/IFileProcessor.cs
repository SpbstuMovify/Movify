namespace ChunkerService.FileProcessing.FileProcessors;

public interface IFileProcessor
{
    FileProcessorType Type { get; }

    Task ProcessAsync(
        FileProcessorRequest request,
        IServiceScope scope,
        CancellationToken cancellationToken
    );
}
