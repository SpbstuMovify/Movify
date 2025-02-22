using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ChunkerService.Grpc.Interceptors;

public class ValidationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        var validationErrors = ValidateRequest(request);

        if (validationErrors.Count != 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Incorrect request data:" + string.Join("; ", validationErrors)));
        }

        return await continuation(request, context);
    }

    private static List<string> ValidateRequest<TRequest>(TRequest request)
    {
        var errors = new List<string>();

        switch (request)
        {
            case Movify.ProcessVideoRequest processVideRequest:
            {
                if (string.IsNullOrWhiteSpace(processVideRequest.BaseUrl))
                {
                    errors.Add("BaseUrl cannot be null or empty");
                }

                if (string.IsNullOrWhiteSpace(processVideRequest.BucketName))
                {
                    errors.Add("BucketName cannot be null or empty");
                }

                if (string.IsNullOrWhiteSpace(processVideRequest.Key))
                {
                    errors.Add("Key cannot be null or empty");
                }

                break;
            }

            default: errors.Add("Unsupported operation"); break;
        }

        return errors;
    }
}
