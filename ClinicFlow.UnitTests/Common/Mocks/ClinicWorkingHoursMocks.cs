using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class ClinicWorkingHoursMocks
    {
        public static Mock<IClinicWorkingHourRepository> ClinicWorkingHourRepository()
            => new Mock<IClinicWorkingHourRepository>();
    }
}