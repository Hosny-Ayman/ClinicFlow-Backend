using ClinicFlow.Application.Common.Authentication;
using FluentValidation.AspNetCore;

namespace ClinicFlow.Api.Extensions
{
    public static class ApiRegistration
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddApiBehavior();

            services.AddFluentValidationAutoValidation();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            return services;
        }
    }
}
