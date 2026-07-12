using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Authentication;
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
            services.AddScoped<AuthenticationService>();


            services.AddAutoMapper(typeof(ApplicationRegistration).Assembly);

            services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

            return services;
        }


    }
}
