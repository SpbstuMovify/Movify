using MediaService.Grpc;
using MediaService.Utils.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movify;
using System.Security.Claims;
using System.Text;

namespace MediaService.Utils;

public static class Extentions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("jwt");
        var options = section.Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration not configured.");
        var key = Encoding.UTF8.GetBytes(options.Secret ?? throw new InvalidOperationException("JWT secret not configured."));

        services.Configure<JwtOptions>(section);
        services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
        {
            var address = configuration["GrpcClientSettings:AuthServiceAddress"];
            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("AuthServiceAddress is not configured.");
            }
            o.Address = new Uri(address);
        });
        services.AddTransient<IAuthGrpcClient, AuthGrpcClient>();
        services.AddTransient<JwtMiddleware>();

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = "userRole"
                };
            });

        services.AddAuthorization(x =>
        {
            x.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
    }
}