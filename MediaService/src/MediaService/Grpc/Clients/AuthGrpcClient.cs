using MediaService.Dtos.Claims;

namespace MediaService.Grpc.Clients;

public class AuthGrpcClient(Movify.AuthService.AuthServiceClient client) : IAuthGrpcClient
{
    public async Task<ClaimsDto> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token cannot be null or empty", nameof(token));
        
        var response = await client.ValidateTokenAsync(
            new Movify.ValidationTokenRequest
            {
                Token = token
            }
        );

        return new ClaimsDto(response.Email, response.Role);
    }
}
