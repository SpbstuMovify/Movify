using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ChunkerService.Dtos.Chunker;
using ChunkerService.Services;
using ChunkerService.Hls;

namespace ChunkerService.Grpc;

public class ChunkerGrpcServer(ILogger<ChunkerGrpcServer> logger, IChunkerService chunkerService, IHlsCreator hlsCreator) : Movify.ChunkerService.ChunkerServiceBase
{
    public override Task<Empty> ProcessVideo(Movify.ProcessVideoRequest request, ServerCallContext context)
    {
        try
        {
            chunkerService.ProcessVideo(new ProcessVideoDto
            {
                BucketName = request.BucketName,
                Key = request.Key,
                BaseUrl = request.BaseUrl
            });
        }
        catch (Exception e)
        {
            logger.LogWarning($"Something went wrong while processing ValidateToken: {e.Message}");
        }

        return Task.FromResult(new Empty());
    }

    public override Task<Empty> CancelVideoProcessing(Movify.CancelVideoProcessingRequest request, ServerCallContext context)
    {
        hlsCreator.CancelToken(Guid.Parse(request.TokenGuid));
        return Task.FromResult(new Empty());
    }
}
