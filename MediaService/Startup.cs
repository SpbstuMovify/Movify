using Amazon.S3;
using MediaService.Repositories;
using MediaService.Services;
using MediaService.Utils.FileProcessing;
using MediaService.Utils.Middleware;

namespace MediaService;
public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IBucketRepository, BucketRepository>();
        services.AddSingleton<IFileProcessingQueue, FileProcessingQueue>();
        services.AddScoped<IBucketService, BucketService>();
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

        app.UseAuthentication();
        logger.LogInformation("Authentication configured");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
