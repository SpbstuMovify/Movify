namespace ChunkerService.FileProcessing.FileProcessors;

public interface IFileProcessorFactory
{
    IFileProcessor GetProcessor(FileProcessorType destination);
}
