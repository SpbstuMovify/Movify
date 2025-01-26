using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using MediaService.Grpc;
using MediaService.Utils.Handles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Movify;

namespace MediaService.Utils.Configuration;

public static class Extentions
{
    public static void AddGrpcClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcClientOptions>(configuration.GetSection("GrpcClient"));

        services.AddGrpcClient<AuthService.AuthServiceClient>((sp, o) =>
        {
            var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            o.Address = new Uri(grpcClientOptions.AuthServiceUrl);
        });
        services.AddTransient<IAuthGrpcClient, AuthGrpcClient>();

        services.AddGrpcClient<ChunkerService.ChunkerServiceClient>((sp, o) =>
        {
            var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            o.Address = new Uri(grpcClientOptions.ChunkerServiceUrl);
        });
        services.AddScoped<IChunckerGrpcClient, ChunckerGrpcClient>();

        services.AddGrpcClient<ContentService.ContentServiceClient>((sp, o) =>
        {
            var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
            o.Address = new Uri(grpcClientOptions.ContentServiceUrl);
        });
        services.AddScoped<IContentGrpcClient, ContentGrpcClient>();
    }

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = ExternalAuthenticationHandler.SchemeName;
            options.DefaultChallengeScheme = ExternalAuthenticationHandler.SchemeName;
        }).AddScheme<AuthenticationSchemeOptions, ExternalAuthenticationHandler>(ExternalAuthenticationHandler.SchemeName, o => { });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(ExternalAuthenticationHandler.SchemeName)
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    public static void AddAws(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsOptions>(configuration.GetSection("AWS"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsOptions = sp.GetRequiredService<IOptions<AwsOptions>>().Value;

            AWSCredentials credentials;

            var sharedFile = new SharedCredentialsFile();
            if (!sharedFile.TryGetProfile("default", out var profile) || !AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out credentials))
            {
                var accessKey = awsOptions.AccessKey ?? throw new InvalidOperationException("AccessKey is not configured");
                var secretKey = awsOptions.SecretKey ?? throw new InvalidOperationException("SecretKey is not configured");
                credentials = new BasicAWSCredentials(accessKey, secretKey);
            }

            var region = awsOptions.Region;
            var serviceUrl = awsOptions.ServiceUrl;
            var usePathStyle = awsOptions.UsePathStyle;

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                ServiceURL = serviceUrl,
                ForcePathStyle = usePathStyle
            };

            return new AmazonS3Client(credentials, s3Config);
        });
    }
}
