using FluentValidation;
using FluentValidation.AspNetCore;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;
using MediaService.FileProcessing;
using MediaService.FileProcessing.FileProcessors;
using MediaService.Grpc;
using MediaService.Repositories;
using MediaService.Services;
using MediaService.Utils.Configuration;
using MediaService.Utils.Middleware;

using Microsoft.AspNetCore.Http.Features;

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

builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 10L * 1024 * 1024 * 1024; });

builder.Services.AddControllers();
builder.Services.AddGrpc();

builder.Services.AddGrpcClient(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAws(builder.Configuration);

builder.Services.AddScoped<IValidator<CreateBucketRequest>, CreateBucketRequestValidator>();
builder.Services.AddScoped<IValidator<CreateFileRequest>, CreateFileRequestValidator>();
builder.Services.AddScoped<IValidator<DeleteBucketRequest>, DeleteBucketRequestValidator>();
builder.Services.AddScoped<IValidator<DeleteFileRequest>, DeleteFileRequestValidator>();
builder.Services.AddScoped<IValidator<GetFileRequest>, GetFileRequestValidator>();
builder.Services.AddScoped<IValidator<GetFilesRequest>, GetFilesRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateFileRequest>, UpdateFileRequestValidator>();

builder.Services.AddScoped<IChunkerCallbackService, ChunkerCallbackService>();
builder.Services.AddScoped<IBucketRepository, BucketRepository>();
builder.Services.AddScoped<IBucketService, BucketService>();

builder.Services.AddTransient<IFileProcessor, InternalFileProcessor>();
builder.Services.AddTransient<IFileProcessor, ContentImageFileProcessor>();
builder.Services.AddTransient<IFileProcessor, EpisodeVideoFileProcessor>();
builder.Services.AddSingleton<IFileProcessorFactory, FileProcessorFactory>();

builder.Services.AddSingleton<IFileProcessingQueue, FileProcessingQueue>();
builder.Services.AddHostedService<FileProcessingService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

var logger = app.Logger;

logger.LogInformation("Starting application configuration");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    logger.LogInformation("Development environment detected");
}

app.UseRouting();
logger.LogInformation("Routing configured");

app.UseMiddleware<ExceptionHandlingMiddleware>();
logger.LogInformation("ExceptionHandlingMiddleware configured");

app.UseMiddleware<AuthorizationHandlingMiddleware>();
logger.LogInformation("AuthorizationHandlingMiddleware configured");

app.UseAuthentication();
logger.LogInformation("Authentication configured");

app.UseAuthorization();
logger.LogInformation("Authorization configured");

app.MapControllers();
app.MapGrpcService<MediaGrpcServer>();
logger.LogInformation("Endpoints mapped");

app.Run();
