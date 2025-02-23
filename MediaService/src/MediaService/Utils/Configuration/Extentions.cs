using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;

using MediaService.Grpc.Clients;
using MediaService.Utils.Handlers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Movify;

namespace MediaService.Utils.Configuration;

public static class Extensions
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

        services.AddGrpcClient<AuthService.AuthServiceClient>(
            (
                sp,
                o
            ) =>
            {
                var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
                o.Address = new Uri(grpcClientOptions.AuthServiceUrl);
            }
        );
        services.AddTransient<IAuthGrpcClient, AuthGrpcClient>();

        services.AddGrpcClient<ChunkerService.ChunkerServiceClient>(
            (
                sp,
                o
            ) =>
            {
                var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
                o.Address = new Uri(grpcClientOptions.ChunkerServiceUrl);
            }
        );
        services.AddScoped<IChunckerGrpcClient, ChunkerGrpcClient>();

        services.AddGrpcClient<ContentService.ContentServiceClient>(
            (
                sp,
                o
            ) =>
            {
                var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
                o.Address = new Uri(grpcClientOptions.ContentServiceUrl);
            }
        );
        services.AddScoped<IContentGrpcClient, ContentGrpcClient>();
    }

    public static void AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = ExternalAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = ExternalAuthenticationHandler.SchemeName;
            }
        ).AddScheme<AuthenticationSchemeOptions, ExternalAuthenticationHandler>(ExternalAuthenticationHandler.SchemeName, _ => { });

        services.AddAuthorization(
            options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                        .AddAuthenticationSchemes(ExternalAuthenticationHandler.SchemeName)
                                        .RequireAuthenticatedUser()
                                        .Build();
            }
        );
    }

    public static void AddAws(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<AwsOptions>()
                .Bind(configuration.GetSection(AwsOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IAmazonS3>(
            sp =>
            {
                var awsOptions = sp.GetRequiredService<IOptions<AwsOptions>>().Value;

                var credentials = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);

                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(awsOptions.Region),
                    ServiceURL = awsOptions.ServiceUrl,
                    ForcePathStyle = awsOptions.UsePathStyle
                };

                return new AmazonS3Client(credentials, s3Config);
            }
        );
    }
}
