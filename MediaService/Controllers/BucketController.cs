using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Controllers;


[Route("api/v1/bucket")]
[ApiController]
public class BucketController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return await Task.FromResult(Ok());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromQuery(Name = "bucket-name")] string bucketName
    )
    {
        return await Task.FromResult(Created());
    }

    [HttpDelete("{bucket-name}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        [FromRoute(Name = "bucket-name")] string bucketName
    )
    {
        return await Task.FromResult(NoContent());
    }

    [HttpGet("{bucket-name}/files")]
    [Authorize]
    public async Task<IActionResult> GetAllFiles(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string prefix
    )
    {
        return await Task.FromResult(Ok());
    }

    [HttpPost("{bucket-name}/files")]
    [Authorize]
    public async Task<IActionResult> CreateFile(
        [FromBody] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromQuery(Name = "prefix")] string prefix,
        [FromQuery(Name = "proc-video")] bool isVideoProcNecessary
    )
    {
        return await Task.FromResult(Ok());
    }

    [HttpGet("{bucket-name}/files/{file-name}")]
    public async Task<IActionResult> GetFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "file-name")] string fileName
    )
    {
        return await Task.FromResult(Ok());
    }

    [HttpPut("{bucket-name}/files/{file-name}")]
    [Authorize]
    public async Task<IActionResult> UpdateFile(
        [FromBody] IFormFile file,
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "file-name")] string fileName,
        [FromQuery(Name = "proc-video")] bool isVideoProcNecessary
    )
    {
        return await Task.FromResult(Ok());
    }

    [HttpDelete("{bucket-name}/files/{file-name}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile(
        [FromRoute(Name = "bucket-name")] string bucketName,
        [FromRoute(Name = "file-name")] string fileName
    )
    {
        return await Task.FromResult(NoContent());
    }

}
