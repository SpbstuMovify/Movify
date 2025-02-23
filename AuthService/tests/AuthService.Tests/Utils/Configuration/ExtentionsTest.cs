using AuthService.Grpc;
using AuthService.Utils.Configuration;
using AuthService.Utils.Jwt;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthService.Tests.Utils.Configuration;

[TestSubject(typeof(Extentions))]
public class ExtensionsTest
{
    [Fact]
    public void AddGrpcClient_RegistersServicesAndValidOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "GrpcClient:ContentServiceUrl", "https://localhost:5001" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection(inMemorySettings)
                                       .Build();

        // Act
        services.AddGrpcClient(configuration);
        var provider = services.BuildServiceProvider();

        // Assert: Проверяем, что опции корректно привязались и прошли валидацию
        var grpcOptions = provider.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
        Assert.Equal("https://localhost:5001", grpcOptions.ContentServiceUrl);

        // Assert: Проверяем регистрацию IContentGrpcClient
        var contentClient = provider.GetService<IContentGrpcClient>();
        Assert.NotNull(contentClient);
    }

    [Fact]
    public void AddGrpcClient_InvalidConfiguration_ThrowsValidationException()
    {
        // Arrange: Отсутствует обязательное значение ContentServiceUrl
        var services = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection(new Dictionary<string, string?>())
                                       .Build();

        // Act
        services.AddGrpcClient(configuration);

        // Assert: При доступе к опциям должно быть выброшено исключение валидации
        Assert.Throws<OptionsValidationException>(
            () =>
            {
                var provider = services.BuildServiceProvider(validateScopes: true);
                // Триггерим валидацию, получив опции
                _ = provider.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            }
        );
    }

    [Fact]
    public void AddJwt_RegistersServicesAndValidOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "abcdefghijklmnop" },
            { "Jwt:ExpirySeconds", "7200" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection(inMemorySettings)
                                       .Build();

        // Act
        services.AddJwt(configuration);
        var provider = services.BuildServiceProvider();

        // Assert: Проверяем, что опции корректно привязались и прошли валидацию
        var jwtOptions = provider.GetRequiredService<IOptions<JwtOptions>>().Value;
        Assert.Equal("abcdefghijklmnop", jwtOptions.Secret);
        Assert.Equal(7200, jwtOptions.ExpirySeconds);

        // Assert: Проверяем, что IJwtBuilder зарегистрирован как singleton
        var jwtBuilder1 = provider.GetRequiredService<IJwtBuilder>();
        var jwtBuilder2 = provider.GetRequiredService<IJwtBuilder>();
        Assert.Same(jwtBuilder1, jwtBuilder2);
    }

    [Fact]
    public void AddJwt_InvalidConfiguration_ThrowsValidationException()
    {
        // Arrange: Отсутствует обязательное значение Jwt:Secret
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:ExpirySeconds", "7200" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection(inMemorySettings)
                                       .Build();

        // Act
        services.AddJwt(configuration);

        // Assert: При доступе к опциям должно быть выброшено исключение валидации
        Assert.Throws<OptionsValidationException>(
            () =>
            {
                var provider = services.BuildServiceProvider(validateScopes: true);
                _ = provider.GetRequiredService<IOptions<JwtOptions>>().Value;
            }
        );
    }
}
