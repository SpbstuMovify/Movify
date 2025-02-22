using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ChunkerService.Grpc.Interceptors;

public class ErrorHandlerInterceptor(ILogger<ErrorHandlerInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            logger.LogWarning(ex, $"RpcException status code: {ex.StatusCode}, detail: {ex.Status.Detail} in gRPC call");
            throw new RpcException(new Status(ex.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in gRPC call");
            throw new RpcException(new Status(StatusCode.Internal, "Unexpected server error"));
        }
    }
}
