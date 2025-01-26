using MediaService.Dtos.Claims;

namespace MediaService.Grpc;

public interface IAuthGrpcClient
{
    Task<ClaimsDto> ValidateToken(string token);
}
