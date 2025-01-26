using MediaService.Dtos.Chunker;

namespace MediaService.Services;

public interface IChunkerCallbackService
{
    Task OnSuccess(ProcessVideoDtoCallbackSuccessDto successDto);
    Task OnFailed(ProcessVideoDtoCallbackFailedDto failedDto);
}
