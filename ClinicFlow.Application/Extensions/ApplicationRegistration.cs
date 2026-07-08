using ClinicFlow.Application.Features.Clinics;
using ClinicFlow.Application.Features.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Application.Extensions
{
    public static class ApplicationRegistration
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ClinicService>();
            services.AddScoped<UserService>();




            services.AddAutoMapper(typeof(ApplicationRegistration).Assembly);

            services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

            return services;
        }


    }
}
