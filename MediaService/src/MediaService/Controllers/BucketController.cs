using FluentValidation;

using MediaService.Controllers.Requests;
using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.FileProcessing;
using MediaService.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Controllers;

[Route("v1/buckets")]
[ApiController]
public class BucketController(IBucketService bucketService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAll()
    {
        var result = await bucketService.GetBucketsAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        [FromQuery(Name = "bucket-name")] string bucketName,
        [FromServices] IValidator<CreateBucketRequest> validator
    )
    {
        var request = new CreateBucketRequest(bucketName);
        await validator.ValidateAndThrowAsync(request);

        var result = await bucketService.CreateBucketAsync(new CreateBucketDto(request.BucketName));
        return Ok(result);
    }

    [HttpDelete("{bucket-name}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromServices] IValidator<DeleteBucketRequest> validator
    )
    {
        var request = new DeleteBucketRequest(bucketName);
        await validator.ValidateAndThrowAsync(request);

        await bucketService.DeleteBucketAsync(new DeleteBucketDto(request.BucketName));
        return NoContent();
    }

    [HttpGet("{bucket-name}/files")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllFiles(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string? prefix,
        [FromServices] IValidator<GetFilesRequest> validator
    )
    {
        var request = new GetFilesRequest(bucketName, prefix ?? string.Empty);
        await validator.ValidateAndThrowAsync(request);

        var result = await bucketService.GetFilesAsync(new GetFilesInfoDto(request.BucketName, request.Prefix));
        return Ok(result);
    }

    [HttpPost("{bucket-name}/files")]
    [RequestSizeLimit(long.MaxValue)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreateFile(
        [FromForm] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string? prefix,
        [FromQuery(Name = "process")] bool? isVideoProcNecessary,
        [FromQuery(Name = "destination")] FileDestination? destination,
        [FromServices] IValidator<CreateFileRequest> validator
    )
    {
        var request = new CreateFileRequest(
            file,
            bucketName,
            prefix ?? string.Empty,
            isVideoProcNecessary ?? false,
            destination ?? FileDestination.Internal
        );
        await validator.ValidateAndThrowAsync(request);

        var uploadPath = Path.Combine(".tmp", Guid.NewGuid().ToString());
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, request.File.FileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        var result = bucketService.CreateFile(
            new CreateFileInfoDto(
                new UploadedFileInfoDto(
                    filePath,
                    request.File.ContentType,
                    request.File.FileName
                ),
                request.BucketName,
                request.Prefix,
                request.IsVideoProcNecessary,
                request.Destination,
                $"{Request.Path.Value}"
            )
        );

        return Ok(result);
    }

    [HttpGet("{bucket-name}/files/{*key}")]
    public async Task<IActionResult> GetFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key,
        [FromServices] IValidator<GetFileRequest> validator
    )
    {
        var request = new GetFileRequest(bucketName, key);
        await validator.ValidateAndThrowAsync(request);

        var result = await bucketService.GetFileAsync(new GetFileInfoDto(request.BucketName, request.Key));
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPut("{bucket-name}/files/{*key}")]
    [Authorize(Roles = "ADMIN")]
    [RequestSizeLimit(long.MaxValue)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UpdateFile(
        [FromForm] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key,
        [FromQuery(Name = "proc-video")] bool? isVideoProcNecessary,
        [FromQuery(Name = "destination")] FileDestination? destination,
        [FromServices] IValidator<UpdateFileRequest> validator
    )
    {
        var request = new UpdateFileRequest(
            file,
            bucketName,
            key,
            isVideoProcNecessary ?? false,
            destination ?? FileDestination.Internal
        );
        await validator.ValidateAndThrowAsync(request);

        var uploadPath = Path.Combine(".tmp", Guid.NewGuid().ToString());
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, request.File.FileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        var result = bucketService.UpdateFile(
            new UpdateFileInfoDto(
                new UploadedFileInfoDto(
                    filePath,
                    request.File.ContentType,
                    request.File.FileName
                ),
                request.BucketName,
                request.Key,
                request.IsVideoProcNecessary,
                request.Destination,
                $"{Request.Path.Value}"
            )
        );

        return Ok(result);
    }

    [HttpDelete("{bucket-name}/files/{*key}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key,
        [FromServices] IValidator<DeleteFileRequest> validator
    )
    {
        var request = new DeleteFileRequest(bucketName, key);
        await validator.ValidateAndThrowAsync(request);

        await bucketService.DeleteFileAsync(new DeleteFileInfoDto(request.BucketName, request.Key));
        return NoContent();
    }
}
