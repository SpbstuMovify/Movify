using MediaService.Dtos.Chunker;

namespace MediaService.Services;

public interface IChunkerCallbackService
{
    Task OnSuccess(ProcessVideoCallbackSuccessDto successCallbackSuccessDto);
    Task OnFailed(ProcessVideoCallbackFailedDto failedCallbackFailedDto);
}
