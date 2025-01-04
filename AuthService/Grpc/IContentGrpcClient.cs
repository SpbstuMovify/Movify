using Grpc.Core;
using AuthService.Utils;
using AuthService.Services;

namespace AuthService.Grpc;

public interface IContentGrpcClient
{
    Task<string> GetUserRoleAsync(string email);
}
