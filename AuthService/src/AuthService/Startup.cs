using AuthService.Grpc;
using AuthService.Services;
using AuthService.Utils;
using Microsoft.Extensions.Logging;

namespace AuthService;
public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();
        services.AddJwt(Configuration);
        services.AddGrpcClient<Movify.ContentService.ContentServiceClient>(o =>
        {
            var address = Configuration["GrpcClientSettings:ContentServiceAddress"];
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("ContentServiceAddress is not configured.");
            }
            o.Address = new Uri(address);
        });
        services.AddTransient<IEncryptor, Encryptor>();
        services.AddTransient<IContentGrpcClient, ContentGrpcClient>();
        services.AddTransient<IAuthService, AuthService.Services.AuthService>();

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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<AuthGrpcServer>();
            logger.LogInformation("Endpoints mapped");
        });
    }
}
