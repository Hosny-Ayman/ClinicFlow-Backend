using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Infrastructure.Extensions
{
    public static class InfrastructureRegistration
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IUserRepository, UserReposittory>();

            return services;
        }
    }
}
