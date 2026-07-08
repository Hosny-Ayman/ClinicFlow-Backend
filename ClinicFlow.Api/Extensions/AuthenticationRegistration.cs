using System.Text;
using ClinicFlow.Application.Common.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ClinicFlow.Api.Extensions;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddJwtAuthentication( this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;

        services.AddAuthentication(options =>
     {
         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
     })
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters =
             new TokenValidationParameters
             {
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(jwtSettings.Key)),

                 ValidateIssuer = true,
                 ValidIssuer = jwtSettings.Issuer,

                 ValidateAudience = true,
                 ValidAudience = jwtSettings.Audience,

                 ValidateLifetime = true,

                 ClockSkew = TimeSpan.Zero
             };

         options.Events = new JwtBearerEvents
         {
             OnMessageReceived = context =>
             {
                 context.Token =
                     context.Request.Cookies["AccessToken"];

                 return Task.CompletedTask;
             }
         };
     });

        return services;
    }
}