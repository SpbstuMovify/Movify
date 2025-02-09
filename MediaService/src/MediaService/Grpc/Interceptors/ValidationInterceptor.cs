using Grpc.Core;
using Grpc.Core.Interceptors;

namespace MediaService.Grpc.Interceptors;

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
            case Movify.ProcessVideoCallbackRequest processVideoCallbackRequest:
            {
                var baseUrl = processVideoCallbackRequest.BaseUrl;
                var error = processVideoCallbackRequest.Error;
                var bucketName = processVideoCallbackRequest.BucketName;
                var key = processVideoCallbackRequest.Key;
                
                var baseUrlEmpty = string.IsNullOrWhiteSpace(baseUrl);
                var errorEmpty = string.IsNullOrWhiteSpace(error);
                
                if (baseUrlEmpty == errorEmpty)
                {
                    errors.Add(
                        "Exactly one of BaseUrl or Error must be set. " +
                        $"(BaseUrl='{baseUrl}', Error='{error}')"
                    );
                }
                
                if (string.IsNullOrWhiteSpace(bucketName))
                {
                    errors.Add("BucketName cannot be null or empty.");
                }
                
                var splitted = key.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length < 2)
                {
                    errors.Add(
                        $"Key '{key}' is invalid. " +
                        "Expected format: {content_id}/{episode_id}/..."
                    );
                }

                break;
            }

            default: errors.Add("Unsupported operation"); break;
        }

        return errors;
    }
}
