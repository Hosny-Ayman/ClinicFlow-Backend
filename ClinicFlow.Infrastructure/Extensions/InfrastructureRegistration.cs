using ClinicFlow.Application.Common.Authorization;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Interfaces.Jobs;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.Infrastructure.Authentication;
using ClinicFlow.Infrastructure.Data;
using ClinicFlow.Infrastructure.QueryServices;
using ClinicFlow.Infrastructure.Repositories;
using ClinicFlow.Infrastructure.Services.Email;
using ClinicFlow.Infrastructure.Services.Jobs;
using ClinicFlow.Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicFlow.Infrastructure.Extensions
{
    public static class InfrastructureRegistration
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IClinicRepository, ClinicRepository>();
            services.AddScoped<IUserRepository, UserReposittory>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOwnershipService, OwnershipService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserQueryService, UserQueryService>();
            services.AddSingleton<IFileStorageService,CloudinaryFileStorageService>();
            services.AddScoped<ISysteamSettingRepository, SysteamSettingRepository>();
            services.AddScoped<ISysteamSettingService, SysteamSettingService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IClinicQueryService, ClinicQueryService>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IClinicSetupRepository, ClinicSetupRepository>();
            services.AddScoped<IClinicWorkingHourRepository, ClinicWorkingHourRepository>();
            services.AddScoped<IClinicSetupQueryService, ClinicSetupQueryService>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddScoped<ICheckService, CheckService>();
            services.AddScoped<IDoctorQueryService, DoctorQueryService>();
            services.AddScoped<IPatientQueryService, PatientQueryService>();
            services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
            services.AddScoped<IDoctorVacationQueryService, DoctorVacationQueryService>();
            services.AddScoped<IDoctorVacationRepository, DoctorVacationRepository>();
            services.AddScoped<IDoctorVacationJobService, DoctorVacationJobService>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IAppointmentQueryService, AppointmentQueryService>();
            services.AddScoped<IClinicWorkingHoursService, ClinicWorkingHoursService>();
            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IDoctorVacationService, DoctorVacationService>();
            services.AddScoped<IEmailService, BrevoEmailService>();
            services.AddScoped<IAppointmentNotificationJobService, AppointmentNotificationJobService>();



            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();









            return services;
        }
    }
}
