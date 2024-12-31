using AuthMicroservice.Services;
using AuthMicroservice.Utils;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice;
public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddJwt(Configuration);
        services.AddTransient<IEncryptor, Encryptor>();
        services.AddTransient<IAuthService, AuthService>();

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

        app.UseAuthorization();
        logger.LogInformation("Authorization middleware configured");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            logger.LogInformation("Endpoints mapped");
        });
    }
}