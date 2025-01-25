using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.Utils;

public static class Extentions
{
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new JwtOptions();
        var section = configuration.GetSection("jwt");
        section.Bind(options);
        services.Configure<JwtOptions>(section);
        services.AddSingleton<IJwtBuilder, JwtBuilder>();
        services.AddAuthentication()
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret))
                };
            });
    }
}
