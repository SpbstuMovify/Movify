using ChunkerService.Grpc;
using ChunkerService.Hls;
using ChunkerService.Repositories;
using ChunkerService.Services;
using ChunkerService.Utils.Configuration;
using ChunkerService.Utils.FileProcessing;

var builder = WebApplication.CreateBuilder(args);

if (!Directory.Exists(".tmp"))
{
    Directory.CreateDirectory(".tmp");
}

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
builder.Services.AddAws(builder.Configuration);
builder.Services.AddHls(builder.Configuration);

builder.Services.AddScoped<IChunkerRepository, ChunkerRepository>();
builder.Services.AddScoped<IChunkerService, ChunkerService.Services.ChunkerService>();

builder.Services.AddSingleton<IHlsCreator, HlsCreator>();
builder.Services.AddSingleton<IFileProcessingQueue, FileProcessingQueue>();
builder.Services.AddHostedService<FileProcessingService>();

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

app.MapGrpcService<ChunkerGrpcServer>();
logger.LogInformation("Endpoints mapped");

app.Run();
