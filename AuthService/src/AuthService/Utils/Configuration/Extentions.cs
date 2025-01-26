using AuthService.Grpc;
using AuthService.Utils.Jwt;
using Microsoft.Extensions.Options;
using Movify;

namespace AuthService.Utils.Configuration;

public static class Extentions
{
    public static void AddGrpcClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcClientOptions>(configuration.GetSection("GrpcClient"));

        services.AddGrpcClient<ContentService.ContentServiceClient>((sp, o) =>
        {
            var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            o.Address = new Uri(grpcClientOptions.ContentServiceUrl);
        });
        services.AddTransient<IContentGrpcClient, ContentGrpcClient>();
    }

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("JWT"));
        services.AddSingleton<IJwtBuilder, JwtBuilder>();
    }
}
