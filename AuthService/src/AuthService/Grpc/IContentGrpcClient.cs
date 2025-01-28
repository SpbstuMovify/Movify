namespace AuthService.Grpc;

public interface IContentGrpcClient
{
    Task<string> GetUserRoleAsync(string email);
}
