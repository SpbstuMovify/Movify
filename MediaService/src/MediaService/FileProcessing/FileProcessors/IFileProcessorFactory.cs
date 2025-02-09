namespace MediaService.FileProcessing.FileProcessors;

public interface IFileProcessorFactory
{
    IFileProcessor GetProcessor(FileDestination destination);
}
