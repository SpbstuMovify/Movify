using Grpc.Core;
using AuthMicroservice.Utils;
using AuthMicroservice.Services;

namespace AuthMicroservice.Grpc;

public interface IContentGrpcClient
{
    Task<string> GetUserRoleAsync(string email);
}