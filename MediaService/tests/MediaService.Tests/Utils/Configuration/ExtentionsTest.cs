using Amazon.S3;

using JetBrains.Annotations;

using MediaService.Utils.Configuration;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Movify;

namespace MediaService.Tests.Utils.Configuration;

[TestSubject(typeof(Extensions))]
public class ExtensionsTest
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ExtensionsTest()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"GrpcClient:AuthServiceUrl", "https://localhost:5001"},
            {"GrpcClient:ChunkerServiceUrl", "https://localhost:5002"},
            {"GrpcClient:ContentServiceUrl", "https://localhost:5003"},
            {"Aws:AccessKey", "test-access-key"},
            {"Aws:SecretKey", "test-secret-key"},
            {"Aws:Region", "us-west-2"},
            {"Aws:ServiceUrl", "https://s3.amazonaws.com"},
            {"Aws:UsePathStyle", "true"}
        };

        _configuration = new ConfigurationBuilder()
                         .AddInMemoryCollection(inMemorySettings)
                         .Build();

        _services = new ServiceCollection();
    }
    
    [Fact]
    public void AddGrpcClient_RegistersServices_Correctly()
    {
        _services.AddGrpcClient(_configuration);
        var provider = _services.BuildServiceProvider();

        var options = provider.GetService<IOptions<GrpcClientOptions>>();
        Assert.NotNull(options);
        Assert.Equal("https://localhost:5001", options.Value.AuthServiceUrl);
        Assert.Equal("https://localhost:5002", options.Value.ChunkerServiceUrl);
        Assert.Equal("https://localhost:5003", options.Value.ContentServiceUrl);
        
        var authClient = provider.GetService<AuthService.AuthServiceClient>();
        Assert.NotNull(authClient);

        var chunkerClient = provider.GetService<ChunkerService.ChunkerServiceClient>();
        Assert.NotNull(chunkerClient);

        var contentClient = provider.GetService<ContentService.ContentServiceClient>();
        Assert.NotNull(contentClient);
    }
    
    [Fact]
    public void AddJwtAuthentication_RegistersAuthenticationAndAuthorization()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddJwtAuthentication(configuration);
        var provider = services.BuildServiceProvider();

        var authService = provider.GetService<IAuthenticationSchemeProvider>();
        Assert.NotNull(authService);

        var authPolicyProvider = provider.GetService<IAuthorizationPolicyProvider>();
        Assert.NotNull(authPolicyProvider);
    }
    
    [Fact]
    public void AddAws_RegistersS3Client()
    {
        _services.AddAws(_configuration);
        var provider = _services.BuildServiceProvider();

        var options = provider.GetService<IOptions<AwsOptions>>();
        Assert.NotNull(options);
        Assert.Equal("test-access-key", options.Value.AccessKey);
        Assert.Equal("test-secret-key", options.Value.SecretKey);
        Assert.Equal("us-west-2", options.Value.Region);
        Assert.Equal("https://s3.amazonaws.com", options.Value.ServiceUrl);
        Assert.True(options.Value.UsePathStyle);

        var awsOptions = provider.GetService<IOptions<AwsOptions>>();
        Assert.NotNull(awsOptions);
        Assert.Equal("test-access-key", awsOptions.Value.AccessKey);

        var s3Client = provider.GetService<IAmazonS3>();
        Assert.NotNull(s3Client);
    }
}
