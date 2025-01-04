using Movify;
using MediaService.Dtos.Claims;

namespace MediaService.Grpc;

public class AuthGrpcClient(AuthService.AuthServiceClient client) : IAuthGrpcClient
{
    public async Task<ClaimsDto> ValidateToken(string token)
    {
        var response = await client.ValidateTokenAsync(new ValidationTokenRequest
        {
            Token = token
        });

        return new ClaimsDto
        {
            Email = response.Email,
            Role = response.Role
        };
    }
}