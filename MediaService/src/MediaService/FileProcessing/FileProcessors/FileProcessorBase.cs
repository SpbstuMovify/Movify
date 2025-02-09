using MediaService.Dtos.S3;
using MediaService.Repositories;

namespace MediaService.FileProcessing.FileProcessors;

public abstract class FileProcessorBase : IFileProcessor
{
    public abstract FileDestination Destination { get; }

    protected static async Task<S3ObjectDto> UploadFileAsync(
        FileProcessorRequest task,
        IServiceScope scope
    )
    {
        var bucketRepository = scope.ServiceProvider.GetRequiredService<IBucketRepository>();
        return await bucketRepository.UploadFileAsync(task.File, task.BucketName, task.Key);
    }

    public abstract Task ProcessAsync(
        FileProcessorRequest task,
        IServiceScope scope
    );
}
