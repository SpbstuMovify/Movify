using MediaService.Dtos.Claims;

namespace MediaService.Grpc;

public class AuthGrpcClient(Movify.AuthService.AuthServiceClient client) : IAuthGrpcClient
{
    public async Task<ClaimsDto> ValidateToken(string token)
    {
        var response = await client.ValidateTokenAsync(new Movify.ValidationTokenRequest
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