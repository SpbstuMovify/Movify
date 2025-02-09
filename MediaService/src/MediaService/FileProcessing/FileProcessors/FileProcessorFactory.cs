namespace MediaService.FileProcessing.FileProcessors;

public class FileProcessorFactory(IEnumerable<IFileProcessor> processors) : IFileProcessorFactory
{
    public IFileProcessor GetProcessor(FileDestination destination) =>
        processors.FirstOrDefault(p => p.Destination == destination)
        ?? throw new ArgumentException($"No processor to assign {destination}");
}
