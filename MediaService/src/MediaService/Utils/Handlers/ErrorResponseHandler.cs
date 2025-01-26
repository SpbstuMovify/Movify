using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MediaService.Utils.Handles;

public static class ErrorResponseHandler
{
    public static Task HandleErrorAsync(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var headers = new { };

        var body = new
        {
            type = "about:blank",
            title = ToTitleCaseWithSpaces(statusCode),
            status = (int)statusCode,
            detail
        };

        var response = new
        {
            statusCode = ToSnakeCaseUpper(statusCode),
            headers,
            typeMessageCode = "",
            titleMessageCode = "",
            detailMessageCode = "",
            detailMessageArguments = (object?)null,
            body
        };

        var json = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(json);
    }

    private static string ToSnakeCaseUpper(HttpStatusCode statusCode)
    {
        string original = statusCode.ToString();
        string snakeCase = Regex.Replace(original, "([a-z])([A-Z])", "$1_$2");
        return snakeCase.ToUpper();
    }

    private static string ToTitleCaseWithSpaces(HttpStatusCode statusCode)
    {
        string original = statusCode.ToString();
        return Regex.Replace(original, "([a-z])([A-Z])", "$1 $2");
    }
}
