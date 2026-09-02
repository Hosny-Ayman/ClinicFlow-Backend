using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class DoctorMocks
    {
        public static Mock<IUserRoleRepository> UserRoleRepository() => new Mock<IUserRoleRepository>();

        public static Mock<IFileStorageService> FileStorageService() => new Mock<IFileStorageService>();

        public static Mock<IDoctorQueryService> QueryService() => new Mock<IDoctorQueryService>();

        public static Mock<IDoctorScheduleService> DoctorScheduleService() => new Mock<IDoctorScheduleService>();

        public static Mock<IDoctorRepository> DoctorRepository() => new Mock<IDoctorRepository>();

        public static Mock<IUserRepository> UserRepository() => new Mock<IUserRepository>();

        public static Mock<ICheckService> CheckService() => new Mock<ICheckService>();

        public static Mock<IUserQueryService> UserQueryService() => new Mock<IUserQueryService>();

        public static Mock<ISpecialtyRepository> SpecialtyRepository() => new Mock<ISpecialtyRepository>();
    }
}