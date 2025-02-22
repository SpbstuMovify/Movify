namespace ChunkerService.FileProcessing.FileProcessors;

public class FileProcessorFactory(IEnumerable<IFileProcessor> processors) : IFileProcessorFactory
{
    public IFileProcessor GetProcessor(FileProcessorType type) =>
        processors.FirstOrDefault(p => p.Type == type)
        ?? throw new ArgumentException($"No processor to assign {type}");
}
