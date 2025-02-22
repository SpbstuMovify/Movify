using Amazon.S3;

using ChunkerService.FileProcessing.FileProcessors;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Services;
using ChunkerService.Utils.Configuration;
using ChunkerService.Utils.ProcessRunners;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Movify;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(Extensions))]
public class ExtensionsTest
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ExtensionsTest()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "GrpcClient:MediaServiceUrl", "https://localhost:5000" },

            { "Aws:AccessKey", "test-access-key" },
            { "Aws:SecretKey", "test-secret-key" },
            { "Aws:Region", "us-west-2" },
            { "Aws:ServiceUrl", "https://s3.amazonaws.com" },
            { "Aws:UsePathStyle", "true" },

            { "Hls:FfmpegPath", @"C:\ffmpeg\bin\ffmpeg.exe" },
            { "Hls:SegmentDuration", "10" },
            { "Hls:AudioBitrate", "128" },
            { "Hls:AdditionalFfmpegArgs", "-arg" },

            { "Hls:Variants:0:Name", "Variant1" },
            { "Hls:Variants:0:Width", "1920" },
            { "Hls:Variants:0:Height", "1080" },
            { "Hls:Variants:0:VideoBitrate", "5000" }
        };

        _configuration = new ConfigurationBuilder()
                         .AddInMemoryCollection(inMemorySettings)
                         .Build();

        _services = new ServiceCollection();
    }

    [Fact]
    public void AddGrpcClient_RegistersMediaClient_Correctly()
    {
        // Act
        _services.AddGrpcClient(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<GrpcClientOptions>>();
        Assert.NotNull(options);
        Assert.Equal("https://localhost:5000", options.Value.MediaServiceUrl);

        Assert.NotNull(provider.GetService<MediaService.MediaServiceClient>());
        Assert.NotNull(provider.GetService<IMediaGrpcClient>());
    }

    [Fact]
    public void AddAws_RegistersS3Client_Correctly()
    {
        // Act
        _services.AddAws(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<AwsOptions>>();
        Assert.NotNull(options);
        Assert.Equal("test-access-key", options.Value.AccessKey);
        Assert.Equal("test-secret-key", options.Value.SecretKey);
        Assert.Equal("us-west-2", options.Value.Region);
        Assert.Equal("https://s3.amazonaws.com", options.Value.ServiceUrl);
        Assert.True(options.Value.UsePathStyle);

        Assert.NotNull(provider.GetService<IAmazonS3>());
    }

    [Fact]
    public void AddHls_RegistersHlsCreator_Correctly()
    {
        // Act
        _services.AddLogging();
        _services.AddSingleton<IFileService, FileService>();
        _services.AddSingleton<IProcessRunner, FfmpegProcessRunner>();
        _services.AddHls(_configuration);
        var provider = _services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<HlsOptions>>();
        Assert.NotNull(options);
        Assert.Equal(@"C:\ffmpeg\bin\ffmpeg.exe", options.Value.FfmpegPath);
        Assert.Equal(10, options.Value.SegmentDuration);
        Assert.Equal(128, options.Value.AudioBitrate);
        Assert.Equal("-arg", options.Value.AdditionalFfmpegArgs);

        Assert.NotNull(options.Value.Variants);
        Assert.Single(options.Value.Variants);
        Assert.Equal("Variant1", options.Value.Variants[0].Name);
        Assert.Equal(1920, options.Value.Variants[0].Width);
        Assert.Equal(1080, options.Value.Variants[0].Height);
        Assert.Equal(5000, options.Value.Variants[0].VideoBitrate);

        Assert.NotNull(provider.GetService<IHlsCreator>());
        Assert.NotNull(provider.GetService<IFileProcessor>());
    }
}
