namespace AuthService.Grpc;

public class ContentGrpcClient(
    ILogger<ContentGrpcClient> logger,
    Movify.ContentService.ContentServiceClient contentClient
) : IContentGrpcClient
{
    public Task<string> GetUserRoleAsync(string email)
    {
        logger.LogInformation("Getting user role");

        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return GetUserRoleInternalAsync(email);
    }

    private async Task<string> GetUserRoleInternalAsync(string email)
    {
        var response = await contentClient
            .GetUserRoleAsync(new Movify.UserRoleRequest { Email = email })
            .ConfigureAwait(false);

        return response.Role;
    }
}
