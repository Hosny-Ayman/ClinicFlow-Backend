using ClinicFlow.Application.Common.Configurations;
using System.Runtime.CompilerServices;

namespace ClinicFlow.Api.Extensions
{
    public static class InfrastructureRegistration
    {

        public static IServiceCollection AddInfrastructureServices (this IServiceCollection services , IConfiguration configuration)
        {

            return services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        }
        



    }
}
