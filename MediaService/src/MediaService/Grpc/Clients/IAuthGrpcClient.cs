using MediaService.Dtos.Claims;

namespace MediaService.Grpc.Clients;

public interface IAuthGrpcClient
{
    Task<ClaimsDto> ValidateToken(string token);
}
