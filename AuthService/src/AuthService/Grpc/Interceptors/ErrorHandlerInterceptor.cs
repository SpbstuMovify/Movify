﻿using AuthService.Utils.Exceptions;

using Grpc.Core;
using Grpc.Core.Interceptors;

namespace AuthService.Grpc.Interceptors;

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
        catch (AuthServiceException ex)
        {
            logger.LogWarning(ex, "AuthServiceException in gRPC call");
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            logger.LogWarning(ex, $"RpcException status code: {ex.StatusCode}, detail: {ex.Status.Detail} in gRPC call");
            throw new RpcException(new Status(ex.StatusCode, ex.Message));
        }
        catch (RpcException ex)
        {
            logger.LogWarning(ex, $"RpcException status code: {ex.StatusCode}, detail: {ex.Status.Detail} in gRPC call");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in gRPC call");
            throw new RpcException(new Status(StatusCode.Internal, "Unexpected server error"));
        }
    }
}
