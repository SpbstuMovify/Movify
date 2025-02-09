using System.Net;
using System.Text.Json;

using JetBrains.Annotations;

using MediaService.Utils.Handlers;

using Microsoft.AspNetCore.Http;

namespace MediaService.Tests.Utils.Handlers;

[TestSubject(typeof(ErrorResponseHandler))]
public class ErrorResponseHandlerTest
{
    [Fact]
    public async Task HandleErrorAsync_WithBadRequest_SetsExpectedJsonResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        var statusCode = HttpStatusCode.BadRequest;
        var detail = "Missing required fields";

        // Act
        await ErrorResponseHandler.HandleErrorAsync(context, statusCode, detail);

        // Assert
        Assert.Equal((int)statusCode, context.Response.StatusCode);

        Assert.Equal("application/json", context.Response.ContentType);

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;
        
        Assert.True(root.TryGetProperty("statusCode", out var statusCodeProp));
        Assert.Equal("BAD_REQUEST", statusCodeProp.GetString());

        Assert.True(root.TryGetProperty("headers", out var headersProp));
        Assert.Equal("{}", headersProp.ToString()); // "headers": {}
        
        Assert.True(root.TryGetProperty("body", out var bodyProp));
        Assert.Equal("about:blank", bodyProp.GetProperty("type").GetString());
        Assert.Equal((int)HttpStatusCode.BadRequest, bodyProp.GetProperty("status").GetInt32());
        Assert.Equal(detail, bodyProp.GetProperty("detail").GetString());
        Assert.Equal("Bad Request", bodyProp.GetProperty("title").GetString());
    }

    [Fact]
    public async Task HandleErrorAsync_WithNotFound_SetsExpectedJsonResponse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        var statusCode = HttpStatusCode.NotFound;
        var detail = "Resource not found";

        // Act
        await ErrorResponseHandler.HandleErrorAsync(context, statusCode, detail);

        // Assert
        Assert.Equal((int)statusCode, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        Assert.Equal("NOT_FOUND", root.GetProperty("statusCode").GetString());
        var bodyProp = root.GetProperty("body");
        Assert.Equal((int)statusCode, bodyProp.GetProperty("status").GetInt32());
        Assert.Equal("Resource not found", bodyProp.GetProperty("detail").GetString());
        Assert.Equal("Not Found", bodyProp.GetProperty("title").GetString());
        Assert.Equal("about:blank", bodyProp.GetProperty("type").GetString());
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError, "INTERNAL_SERVER_ERROR", "Internal Server Error")]
    [InlineData(HttpStatusCode.Forbidden, "FORBIDDEN", "Forbidden")]
    public async Task HandleErrorAsync_VariousStatusCodes_FormatsCorrectly(
        HttpStatusCode status,
        string expectedSnakeCase,
        string expectedTitle
    )
    {
        // Arrange
        var context = new DefaultHttpContext();
        var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        var detail = "Some error detail";

        // Act
        await ErrorResponseHandler.HandleErrorAsync(context, status, detail);

        // Assert
        Assert.Equal((int)status, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = await reader.ReadToEndAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;
        
        var statusCodeStr = root.GetProperty("statusCode").GetString();
        Assert.Equal(expectedSnakeCase, statusCodeStr);

        var bodyProp = root.GetProperty("body");
        Assert.Equal((int)status, bodyProp.GetProperty("status").GetInt32());
        Assert.Equal(detail, bodyProp.GetProperty("detail").GetString());
        Assert.Equal(expectedTitle, bodyProp.GetProperty("title").GetString());
        Assert.Equal("about:blank", bodyProp.GetProperty("type").GetString());
    }
}
