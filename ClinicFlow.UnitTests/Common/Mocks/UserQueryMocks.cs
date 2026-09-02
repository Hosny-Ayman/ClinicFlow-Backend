using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class UserQueryMocks
    {
        public static Mock<IUserQueryService> UserQueryService() => new Mock<IUserQueryService>();
    }
}