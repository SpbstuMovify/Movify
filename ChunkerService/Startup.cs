using Amazon.S3;
using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Repositories;
using ChunkerService.Services;
using ChunkerService.Utils.FileProcessing;

namespace ChunkerService;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();

        services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        services.AddGrpcClient<Movify.MediaService.MediaServiceClient>(o =>
        {
            var address = Configuration["GrpcClientSettings:MediaServiceAddress"];
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("MediaServiceAddress is not configured.");
            }
            o.Address = new Uri(address);
        });

        services.AddScoped<IChunkerRepository, ChunkerRepository>();
        services.AddScoped<IChunkerService, Services.ChunkerService>();

        services.AddSingleton<IHlsCreator, HlsCreator>();
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

        app.UseRouting();
        logger.LogInformation("Routing configured");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<ChunkerGrpcServer>();
            logger.LogInformation("Endpoints mapped");
        });
    }
}
