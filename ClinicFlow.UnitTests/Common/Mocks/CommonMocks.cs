using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Authentication;
using ClinicFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public static class CommonMocks
    {
        public static IMapper Mapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(AuthenticationProfile).Assembly);
            });

            return config.CreateMapper();
        }

        public static Mock<ILogger<T>> Logger<T>() => new Mock<ILogger<T>>();

        public static Mock<IUnitOfWork> UnitOfWork() => new Mock<IUnitOfWork>();

        public static Mock<ICurrentUserService> CurrentUserService() => new Mock<ICurrentUserService>();

    }
}
