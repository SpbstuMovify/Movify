using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediaService.Dtos.Chunker;
using MediaService.Services;

namespace MediaService.Grpc;

public class MediaGrpcServer(ILogger<MediaGrpcServer> logger, IChunkerCallbackService chunkerCallbackService) : Movify.MediaService.MediaServiceBase
{
    public override async Task<Empty> ProcessVideoCallback(Movify.ProcessVideoCallbackRequest request, ServerCallContext context)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Error))
            {
                await chunkerCallbackService.OnSuccess(new ProcessVideoDtoCallbackSuccessDto
                {
                    BucketName = request.BucketName,
                    Key = request.Key,
                    BaseUrl = request.BaseUrl
                });
            }
            else
            {
                logger.LogWarning($"Something went wrong while processing video: {request.Error}");
                await chunkerCallbackService.OnFailed(new ProcessVideoDtoCallbackFailedDto
                {
                    BucketName = request.BucketName,
                    Key = request.Key,
                    Error = request.Error
                });
            }
        }
        catch (Exception e)
        {
            logger.LogWarning($"Something went wrong while processing ValidateToken: {e.Message}");
        }

        return new Empty();
    }
}
