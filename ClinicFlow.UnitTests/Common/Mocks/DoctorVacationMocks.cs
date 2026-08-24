using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class DoctorVacationMocks
    {
        public static Mock<IDoctorVacationRepository> DoctorVacationRepository() => new Mock<IDoctorVacationRepository>();

        public static Mock<IDoctorVacationQueryService> DoctorVacationQueryService() => new Mock<IDoctorVacationQueryService>();
    }
}