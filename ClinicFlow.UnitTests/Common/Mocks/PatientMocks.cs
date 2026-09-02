using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class PatientMocks
    {
        public static Mock<IPatientRepository> PatientRepository()
        {
            return new Mock<IPatientRepository>();
        }

        public static Mock<IPatientQueryService> PatientQueryService()
        {
            return new Mock<IPatientQueryService>();
        }
    }
}
