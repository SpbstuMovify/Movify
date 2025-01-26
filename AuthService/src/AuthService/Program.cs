using AuthService.Grpc;
using AuthService.Services;
using AuthService.Utils.Configuration;
using AuthService.Utils.Encryption;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
}

builder.Services.AddGrpc();
builder.Services.AddGrpcClient(builder.Configuration);

builder.Services.AddJwt(builder.Configuration);
builder.Services.AddTransient<IEncryptor, Encryptor>();

builder.Services.AddTransient<IAuthService, AuthService.Services.AuthService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Starting application configuration");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    logger.LogInformation("Development environment detected");
}

app.MapGrpcService<AuthGrpcServer>();
logger.LogInformation("Endpoints mapped");

app.Run();

