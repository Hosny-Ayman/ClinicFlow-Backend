using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.Api.Extensions
{
    public static class AuthorizationRegistration
    {

        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                foreach (var permission in Enum.GetValues<PermissionEnum>())
                {
                    if (permission == PermissionEnum.None || permission == PermissionEnum.All)
                        continue;


                    options.AddPolicy(
                        permission.ToString(),
                        policy =>
                        {
                            policy.Requirements.Add(
                                new PermissionRequirement(permission)
                            );
                        });
                }
            });

            return services;
        }

    }
}
