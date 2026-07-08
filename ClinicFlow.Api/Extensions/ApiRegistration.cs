using FluentValidation.AspNetCore;

namespace ClinicFlow.Api.Extensions
{
    public static class ApiRegistration
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiBehavior();

            services.AddFluentValidationAutoValidation();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            return services;
        }
    }
}
