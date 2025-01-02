using Grpc.Core;
using AuthMicroservice.Utils;
using AuthMicroservice.Services;

namespace AuthMicroservice.Grpc;

public class ContentGrpcClient(ILogger<ContentGrpcClient> logger, ContentService.ContentServiceClient contentClient) : IContentGrpcClient
{
    public async Task<string> GetUserRoleAsync(string email)
    {
        try
        {
            var response = await contentClient.GetUserRoleAsync(new UserRoleRequest { Email = email });
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
