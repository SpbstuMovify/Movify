using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using ChunkerService.Dtos.Chunker;
using ChunkerService.Services;
using ChunkerService.Hls;

namespace ChunkerService.Grpc;

public class ChunkerGrpcServer(
    ILogger<ChunkerGrpcServer> logger,
    IChunkerService chunkerService,
    IHlsCreator hlsCreator
) : Movify.ChunkerService.ChunkerServiceBase
{
    public override Task<Empty> ProcessVideo(
        Movify.ProcessVideoRequest request,
        ServerCallContext context
    )
    {
        logger.LogInformation("ProcessVideo request received");
        chunkerService.ProcessVideo(new ProcessVideoDto(request.BucketName, request.Key, request.BaseUrl));
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> CancelVideoProcessing(
        Movify.CancelVideoProcessingRequest request,
        ServerCallContext context
    )
    {
        logger.LogInformation("CancelVideoProcessing request received");
        hlsCreator.CancelToken(Guid.Parse(request.TokenGuid));
        return Task.FromResult(new Empty());
    }
}
