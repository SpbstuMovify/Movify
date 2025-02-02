namespace AuthService.Grpc;

public class ContentGrpcClient(
    ILogger<ContentGrpcClient> logger,
    Movify.ContentService.ContentServiceClient contentClient
) : IContentGrpcClient
{
    public async Task<string> GetUserRoleAsync(string email)
    {
        logger.LogInformation("Getting user role");
        
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be null or empty", nameof(email));
        
        var response = await contentClient.GetUserRoleAsync(new Movify.UserRoleRequest { Email = email });
        return response.Role;
    }
}
