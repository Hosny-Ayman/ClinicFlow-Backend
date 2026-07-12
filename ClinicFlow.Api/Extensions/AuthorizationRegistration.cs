using ClinicFlow.Application.Common.Authorization;

namespace ClinicFlow.Api.Extensions
{
    public static class AuthorizationRegistration
    {

        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ManageUsers, policy =>
                {
                    policy.RequireRole("ClinicOwner", "Admin");
                });

                options.AddPolicy(Policies.ManageDoctors, policy =>
                {
                    policy.RequireRole("ClinicOwner");
                });

                options.AddPolicy(Policies.ManageAppointments, policy =>
                {
                    policy.RequireRole("ClinicOwner", "Doctor", "Receptionist");
                });
            });

            return services;
        }

    }
}
