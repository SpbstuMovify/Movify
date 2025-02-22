using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using MediaService.Dtos.Chunker;
using MediaService.Services;

namespace MediaService.Grpc;

public class MediaGrpcServer(
    ILogger<MediaGrpcServer> logger,
    IChunkerCallbackService chunkerCallbackService
) : Movify.MediaService.MediaServiceBase
{
    public override async Task<Empty> ProcessVideoCallback(
        Movify.ProcessVideoCallbackRequest request,
        ServerCallContext context
    )
    {
        if (string.IsNullOrEmpty(request.Error))
        {
            await chunkerCallbackService.OnSuccess(new ProcessVideoCallbackSuccessDto(request.Key, request.BaseUrl));
        }
        else
        {
            logger.LogWarning($"Something went wrong while processing video: {request.Error}");
            await chunkerCallbackService.OnFailed(new ProcessVideoCallbackFailedDto(request.Key));
        }

        return new Empty();
    }
}
