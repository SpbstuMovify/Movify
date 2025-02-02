using Grpc.Core;
using Grpc.Core.Interceptors;

namespace AuthService.Grpc.Interceptors;

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
            case Movify.LoginUserRequest loginRequest:
                if (string.IsNullOrEmpty(loginRequest.Email)) errors.Add("Email is required");
                if (string.IsNullOrEmpty(loginRequest.Role)) errors.Add("Role is required");
                if (string.IsNullOrEmpty(loginRequest.Password)) errors.Add("Password is required");
                if (string.IsNullOrEmpty(loginRequest.PasswordHash)) errors.Add("PasswordHash is required");
                if (string.IsNullOrEmpty(loginRequest.PasswordSalt)) errors.Add("PasswordSalt is required");
                break;

            case Movify.RegisterUserRequest registerRequest:
                if (string.IsNullOrEmpty(registerRequest.Email)) errors.Add("Email is required");
                if (string.IsNullOrEmpty(registerRequest.Role)) errors.Add("Role is required");
                if (string.IsNullOrEmpty(registerRequest.Password)) errors.Add("Password is required");
                break;

            case Movify.ValidationTokenRequest tokenRequest:
                if (string.IsNullOrEmpty(tokenRequest.Token)) errors.Add("Token is required");
                break;

            default: errors.Add("Unsupported operation"); break;
        }

        return errors;
    }
}
