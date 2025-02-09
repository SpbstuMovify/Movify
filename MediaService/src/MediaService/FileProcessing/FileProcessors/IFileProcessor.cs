namespace MediaService.FileProcessing.FileProcessors;

public interface IFileProcessor
{
    FileDestination Destination { get; }

    Task ProcessAsync(
        FileProcessorRequest task,
        IServiceScope scope
    );
}
