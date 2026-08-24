using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class DoctorScheduleMocks
    {
        public static Mock<IDoctorScheduleRepository> DoctorScheduleRepository() => new Mock<IDoctorScheduleRepository>();

        public static Mock<IDoctorRepository> DoctorRepository() => new Mock<IDoctorRepository>();
    }
}