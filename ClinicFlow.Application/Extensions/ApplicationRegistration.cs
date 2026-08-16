using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Authentication;
using ClinicFlow.Application.Features.Clinics;
using ClinicFlow.Application.Features.ClinicSetups;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.Doctors;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.Patients;
using ClinicFlow.Application.Features.Specialties;
using ClinicFlow.Application.Features.SysteamSettings;
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
            services.AddScoped<SysteamSettingService>();
            services.AddScoped<DoctorService>();
            services.AddScoped<PatientService>();
            services.AddScoped<ClinicWorkingHoursService>();
            services.AddScoped<ClinicSetupService>();
            services.AddScoped<SpecialityService>();
            services.AddScoped<CheckService>();
            services.AddScoped<DoctorScheduleService>();
            services.AddAutoMapper(typeof(ApplicationRegistration).Assembly);

            services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

            return services;
        }


    }
}
