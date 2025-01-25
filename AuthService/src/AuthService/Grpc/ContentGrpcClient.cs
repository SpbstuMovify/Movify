using Grpc.Core;

namespace AuthService.Grpc;

public class ContentGrpcClient(ILogger<ContentGrpcClient> logger, Movify.ContentService.ContentServiceClient contentClient) : IContentGrpcClient
{
    public async Task<string> GetUserRoleAsync(string email)
    {
        try
        {
            var response = await contentClient.GetUserRoleAsync(new Movify.UserRoleRequest { Email = email });
            return response.Role;
        }
        catch (RpcException e)
        {
            var status = e.Status;
            logger.LogWarning($"Unable to receive role by email[{email}]: StatusCode[{status.StatusCode}], Detail:[{status.Detail}]");
            throw new Exception(status.Detail);
        }
    }
}
