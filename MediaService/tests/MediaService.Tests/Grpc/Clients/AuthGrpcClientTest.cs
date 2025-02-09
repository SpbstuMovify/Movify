using Grpc.Core;

using JetBrains.Annotations;

using MediaService.Grpc.Clients;

using Moq;

namespace MediaService.Tests.Grpc.Clients;

[TestSubject(typeof(AuthGrpcClient))]
public class AuthGrpcClientTest
{

   private readonly Mock<Movify.AuthService.AuthServiceClient> _authClientMock;
    private readonly AuthGrpcClient _client;

    public AuthGrpcClientTest()
    {
        _authClientMock = new Mock<Movify.AuthService.AuthServiceClient>();
        _client = new AuthGrpcClient(_authClientMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateToken_WithNullOrWhiteSpaceToken_ThrowsArgumentException(string invalidToken)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _client.ValidateToken(invalidToken));

        Assert.Equal("Token cannot be null or empty (Parameter 'token')", exception.Message);
    }

    [Fact]
    public async Task ValidateToken_WithValidToken_SuccessfulGrpcCall_ReturnsClaimsDto()
    {
        // Arrange
        const string testToken = "some-test-token";
        const string expectedEmail = "test@example.com";
        const string expectedRole = "ADMIN";
        
        _authClientMock
            .Setup(client => client.ValidateTokenAsync(
                It.IsAny<Movify.ValidationTokenRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(new AsyncUnaryCall<Movify.ValidationTokenResponse>(
                Task.FromResult(new Movify.ValidationTokenResponse
                {
                    Email = expectedEmail,
                    Role = expectedRole
                }),
                Task.FromResult(new Metadata()),
                () => new Status(StatusCode.OK, string.Empty),
                () => new Metadata(),
                () => { }
            ));

        // Act
        var actualResult = await _client.ValidateToken(testToken);

        // Assert
        Assert.NotNull(actualResult);
        Assert.Equal(expectedEmail, actualResult.Email);
        Assert.Equal(expectedRole, actualResult.Role);
        
        _authClientMock.Verify(
            client => client.ValidateTokenAsync(
                It.Is<Movify.ValidationTokenRequest>(r => r.Token == testToken),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ValidateToken_WithGrpcCallFailure_ThrowsRpcException()
    {
        // Arrange
        const string testToken = "bad-token";
        var expectedStatusCode = StatusCode.Internal;
        var expectedDetail = "Internal server error";
        
        _authClientMock
            .Setup(client => client.ValidateTokenAsync(
                It.IsAny<Movify.ValidationTokenRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ))
            .Throws(new RpcException(new Status(expectedStatusCode, expectedDetail)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(() => _client.ValidateToken(testToken));

        Assert.Equal(expectedStatusCode, ex.StatusCode);
        Assert.Equal(expectedDetail, ex.Status.Detail);

        _authClientMock.Verify(
            client => client.ValidateTokenAsync(
                It.Is<Movify.ValidationTokenRequest>(r => r.Token == testToken),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
