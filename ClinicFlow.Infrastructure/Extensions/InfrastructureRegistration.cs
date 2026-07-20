using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.InterFaces;
using ClinicFlow.Infrastructure.Authentication;
using ClinicFlow.Infrastructure.Data;
using ClinicFlow.Infrastructure.QueryServices;
using ClinicFlow.Infrastructure.Repositories;
using ClinicFlow.Infrastructure.Storage;
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
            services.AddHttpContextAccessor();









            return services;
        }
    }
}
