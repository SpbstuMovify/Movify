using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;

using ChunkerService.FileProcessing.FileProcessors;
using ChunkerService.Grpc;
using ChunkerService.Hls;

using Microsoft.Extensions.Options;

using Movify;

namespace ChunkerService.Utils.Configuration;

public static class Extensions
{
    public static void AddGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<GrpcClientOptions>()
                .Bind(configuration.GetSection(GrpcClientOptions.SectionName))
                .ValidateMiniValidation()
                .ValidateOnStart();

        services.AddGrpcClient<MediaService.MediaServiceClient>(
            (
                sp,
                o
            ) =>
            {
                var grpcClientOptions = sp.GetRequiredService<IOptions<GrpcClientOptions>>().Value;
                o.Address = new Uri(grpcClientOptions.MediaServiceUrl);
            }
        );
        services.AddTransient<IMediaGrpcClient, MediaGrpcClient>();
    }

    public static void AddAws(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<AwsOptions>()
                .Bind(configuration.GetSection(AwsOptions.SectionName))
                .ValidateMiniValidation()
                .ValidateOnStart();

        services.AddSingleton<IAmazonS3>(
            sp =>
            {
                var awsOptions = sp.GetRequiredService<IOptions<AwsOptions>>().Value;

                var sharedFile = new SharedCredentialsFile();
                if (!sharedFile.TryGetProfile("default", out var profile) || !AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials))
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
            }
        );
    }

    public static void AddHls(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<HlsOptions>()
                .Bind(configuration.GetSection(HlsOptions.SectionName))
                .ValidateMiniValidation()
                .ValidateOnStart();

        services.AddSingleton<IHlsCreator, HlsCreator>();
        services.AddTransient<IFileProcessor, HlsFileProcessor>();
    }

    private static OptionsBuilder<TOptions> ValidateMiniValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(new MiniValidationValidateOptions<TOptions>(optionsBuilder.Name));
        return optionsBuilder;
    }
}
