using Amazon.S3;
using MediaService.Repositories;
using MediaService.Services;
using MediaService.Utils;
using MediaService.Utils.FileProcessing;
using MediaService.Utils.Middleware;
using Movify;

namespace MediaService;
public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddGrpc();

        services.AddJwtAuthentication(Configuration);
        
        services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        services.AddGrpcClient<ContentService.ContentServiceClient>(o =>
        {
            var address = Configuration["GrpcClientSettings:ContentServiceAddress"];
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("ContentServiceAddress is not configured.");
            }
            o.Address = new Uri(address);
        });

        services.AddScoped<IBucketRepository, BucketRepository>();
        services.AddScoped<IBucketService, BucketService>();

        services.AddSingleton<IFileProcessingQueue, FileProcessingQueue>();
        services.AddHostedService<FileProcessingService>();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        logger.LogInformation("Starting application configuration");

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            logger.LogInformation("Development environment detected");
        }

        app.UseHttpsRedirection();
        logger.LogInformation("HTTPS redirection enabled");

        app.UseRouting();
        logger.LogInformation("Routing configured");

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        logger.LogInformation("ExceptionHandlingMiddleware configured");

        app.UseMiddleware<JwtMiddleware>();
        logger.LogInformation("JwtMiddleware configured");

        app.UseAuthentication();
        logger.LogInformation("Authentication configured");

        app.UseAuthorization();
        logger.LogInformation("Authorization configured");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
