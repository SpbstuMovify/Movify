using AuthService.Grpc;
using AuthService.Utils.Jwt;

using Microsoft.Extensions.Options;

using Movify;

namespace AuthService.Utils.Configuration;

public static class Extentions
{
    public static void AddGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<GrpcClientOptions>()
                .Bind(configuration.GetSection(GrpcClientOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddGrpcClient<ContentService.ContentServiceClient>(
            (
                serviceProvider,
                options
            ) =>
            {
                var grpcClientOptions = serviceProvider.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
                options.Address = new Uri(grpcClientOptions.ContentServiceUrl);
            }
        );
        services.AddTransient<IContentGrpcClient, ContentGrpcClient>();
    }

    public static void AddJwt(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IJwtBuilder, JwtBuilder>();
    }
}
