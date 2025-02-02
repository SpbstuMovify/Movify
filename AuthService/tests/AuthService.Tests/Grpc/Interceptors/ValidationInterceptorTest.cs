using AuthService.Grpc.Interceptors;

using Grpc.Core;

using JetBrains.Annotations;

namespace AuthService.Tests.Grpc.Interceptors;

[TestSubject(typeof(ValidationInterceptor))]
public class ValidationInterceptorTest
{
    private readonly ValidationInterceptor _interceptor = new();

    #region LoginUserRequest Tests

    [Fact]
    public async Task LoginUserRequest_WithAllFieldsProvided_PassesValidation()
    {
        // Arrange
        var request = new Movify.LoginUserRequest
        {
            Email = "test@example.com",
            Role = "User",
            Password = "pwd",
            PasswordHash = "hash",
            PasswordSalt = "salt"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("token123", response.Token);

        return;

        async Task<Movify.LoginUserResponse> Continuation(
            Movify.LoginUserRequest req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult(new Movify.LoginUserResponse { Token = "token123" });
        }
    }

    [Fact]
    public async Task LoginUserRequest_WithMissingFields_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.LoginUserRequest
        {
            Email = "",
            Role = "",
            Password = "",
            PasswordHash = "",
            PasswordSalt = ""
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Email is required", ex.Status.Detail);
        Assert.Contains("Role is required", ex.Status.Detail);
        Assert.Contains("Password is required", ex.Status.Detail);
        Assert.Contains("PasswordHash is required", ex.Status.Detail);
        Assert.Contains("PasswordSalt is required", ex.Status.Detail);
        
        return;

        Task<Movify.LoginUserResponse> Continuation(
            Movify.LoginUserRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult(new Movify.LoginUserResponse { Token = "fake_token" });
        }
    }

    #endregion

    #region RegisterUserRequest Tests

    [Fact]
    public async Task RegisterUserRequest_WithAllFieldsProvided_PassesValidation()
    {
        // Arrange
        var request = new Movify.RegisterUserRequest
        {
            Email = "new@example.com",
            Role = "User",
            Password = "secret"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("register_token", response.Token);
        Assert.Equal("hash123", response.PasswordHash);
        Assert.Equal("salt123", response.PasswordSalt);

        return;

        async Task<Movify.RegisterUserResponse> Continuation(
            Movify.RegisterUserRequest req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult(
                new Movify.RegisterUserResponse
                {
                    Token = "register_token",
                    PasswordHash = "hash123",
                    PasswordSalt = "salt123"
                }
            );
        }
    }

    [Fact]
    public async Task RegisterUserRequest_WithMissingFields_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.RegisterUserRequest
        {
            Email = "",
            Role = "",
            Password = ""
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Email is required", ex.Status.Detail);
        Assert.Contains("Role is required", ex.Status.Detail);
        Assert.Contains("Password is required", ex.Status.Detail);

        return;

        Task<Movify.RegisterUserResponse> Continuation(
            Movify.RegisterUserRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult(new Movify.RegisterUserResponse { Token = "fake" });
        }
    }

    #endregion

    #region ValidationTokenRequest Tests

    [Fact]
    public async Task ValidationTokenRequest_WithTokenProvided_PassesValidation()
    {
        // Arrange
        var request = new Movify.ValidationTokenRequest
        {
            Token = "valid_jwt"
        };

        var context = new TestServerCallContext();

        // Act
        var response = await _interceptor.UnaryServerHandler(request, context, Continuation);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("test@example.com", response.Email);
        Assert.Equal("User", response.Role);
        
        return;

        async Task<Movify.ValidationTokenResponse> Continuation(
            Movify.ValidationTokenRequest req,
            ServerCallContext ctx
        )
        {
            return await Task.FromResult(
                new Movify.ValidationTokenResponse
                {
                    Email = "test@example.com",
                    Role = "User"
                }
            );
        }
    }

    [Fact]
    public async Task ValidationTokenRequest_WithMissingToken_ThrowsRpcException()
    {
        // Arrange
        var request = new Movify.ValidationTokenRequest
        {
            Token = ""
        };

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Token is required", ex.Status.Detail);

        return;

        Task<Movify.ValidationTokenResponse> Continuation(
            Movify.ValidationTokenRequest req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult(new Movify.ValidationTokenResponse());
        }
    }

    #endregion

    #region UnsupportedRequest Tests

    [Fact]
    public async Task UnsupportedRequest_ThrowsRpcException()
    {
        // Arrange
        var request = new object();

        var context = new TestServerCallContext();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, Continuation));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
        Assert.Contains("Unsupported operation", ex.Status.Detail);

        return;

        Task<object> Continuation(
            object req,
            ServerCallContext ctx
        )
        {
            return Task.FromResult(new object());
        }
    }

    #endregion
}
