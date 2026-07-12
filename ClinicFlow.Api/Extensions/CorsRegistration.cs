using Microsoft.Extensions.Configuration;

namespace ClinicFlow.Api.Extensions
{
    public static class CorsRegistration
    {
        public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("ClinicFlowCorsPolicy", builder =>
                {
                    if (allowedOrigins is not null && allowedOrigins.Length > 0)
                    {
                        builder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                });
            });

            return services;
        }
    }
}