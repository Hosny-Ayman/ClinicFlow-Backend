using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class ClinicMocks
    {
        public static Mock<IClinicQueryService> ClinicQueryService() => new Mock<IClinicQueryService>();

        public static Mock<IClinicSetupRepository> ClinicSetupRepository() => new Mock<IClinicSetupRepository>();

        public static Mock<IClinicSetupQueryService> ClinicSetupQueryService() => new Mock<IClinicSetupQueryService>();

        public static Mock<IOwnershipService> OwnershipService() => new Mock<IOwnershipService>();

        public static Mock<IClinicRepository> ClinicRepository() => new Mock<IClinicRepository>();
    }
}

