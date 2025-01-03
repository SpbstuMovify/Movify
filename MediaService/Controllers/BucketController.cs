using MediaService.Dtos.Bucket;
using MediaService.Dtos.FileInfo;
using MediaService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Controllers;


[Route("api/v1/bucket")]
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
        [FromQuery(Name = "bucket-name")] string bucketName
    )
    {
        var result = await bucketService.CreateBucketAsync(new CreateBucketDto { Name = bucketName });
        return Ok(result);
    }

    [HttpDelete("{bucket-name}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(
        [FromRoute(Name = "bucket-name")] string bucketName
    )
    {
        await bucketService.DeleteBucketAsync(new DeleteBucketDto { Name = bucketName });
        return NoContent();
    }

    [HttpGet("{bucket-name}/files")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllFiles(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string prefix
    )
    {
        var result = await bucketService.GetFilesAsync(new GetFilesInfoDto
        {
            BucketName = bucketName,
            Prefix = prefix
        });
        return Ok(result);
    }

    [HttpPost("{bucket-name}/files")]
    [Authorize(Roles = "ADMIN")]
    public IActionResult CreateFile(
        [FromBody] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string prefix,
        [FromQuery(Name = "proc-video")] bool isVideoProcNecessary
    )
    {
        var result = bucketService.CreateFile(file, new CreateFileInfoDto
        {
            BucketName = bucketName,
            Prefix = prefix,
            IsVideoProcNecessary = isVideoProcNecessary
        });
        return Ok(result);
    }

    [HttpGet("{bucket-name}/files/{*key}")]
    public async Task<IActionResult> GetFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key
    )
    {
        var result = await bucketService.GetFileAsync(new GetFileInfoDto { BucketName = bucketName, Key = key });
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPut("{bucket-name}/files/{*key}")]
    [Authorize(Roles = "ADMIN")]
    public IActionResult UpdateFile(
        [FromBody] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key,
        [FromQuery(Name = "proc-video")] bool isVideoProcNecessary
    )
    {
        var result = bucketService.UpdateFile(file, new UpdateFileInfoDto
        {
            BucketName = bucketName,
            Key = key,
            IsVideoProcNecessary = isVideoProcNecessary
        });
        return Ok(result);
    }

    [HttpDelete("{bucket-name}/files/{*key}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "key")] string key
    )
    {
        await bucketService.DeleteFileAsync(new DeleteFileInfoDto
        {
            BucketName = bucketName,
            Key = key
        });
        return NoContent();
    }
}
