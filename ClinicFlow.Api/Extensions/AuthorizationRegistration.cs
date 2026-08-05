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
                    policy.RequireRole("ClinicOwner", "SuperAdmin");
                });

                options.AddPolicy(Policies.ManageDoctors, policy =>
                {
                    policy.RequireRole("ClinicOwner", "SuperAdmin", "Doctor");
                });

                options.AddPolicy(Policies.ManageReceptionist, policy =>
                {
                    policy.RequireRole("ClinicOwner", "SuperAdmin", "Receptionist");
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
