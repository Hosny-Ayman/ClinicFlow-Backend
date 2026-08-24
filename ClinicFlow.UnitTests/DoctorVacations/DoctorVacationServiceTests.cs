using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.DoctorVacations
{
    public class DoctorVacationServiceTests
    {
       

        [Fact]
        public async Task CreateDoctorVacationAsyn_WhenDoctorNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var vacationRepo = DoctorVacationMocks.DoctorVacationRepository();
            var queryService = DoctorVacationMocks.DoctorVacationQueryService();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var mapper = CommonMocks.Mapper();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository(); 

            int clinicId = 1;
            int userId = 100;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync((int?)null);

            var request = DoctorVacationBuilder.CreateRequest(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            var service = new DoctorVacationService(vacationRepo.Object, queryService.Object, unitOfWork.Object, currentUserService.Object, mapper, doctorRepo.Object);

            // Act
            var result = await service.CreateDoctorVacationAsyn(request);

            // Assert
            Assert.False(result.IsSuccess);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never); 
        }

        [Fact]
        public async Task CreateDoctorVacationAsyn_WhenStartDateInFuture_ShouldSetStatusNotStarted_AndSave()
        {
            // Arrange
            var vacationRepo = DoctorVacationMocks.DoctorVacationRepository();
            var queryService = DoctorVacationMocks.DoctorVacationQueryService();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var mapper = CommonMocks.Mapper();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int clinicId = 1;
            int userId = 100;
            int expectedDoctorId = 5;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(expectedDoctorId);

            var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
            var request = DoctorVacationBuilder.CreateRequest(userId, futureDate);

            vacationRepo.Setup(repo => repo.AddDoctorVacationAsync(It.IsAny<DoctorVacation>())).Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DoctorVacationService(vacationRepo.Object, queryService.Object, unitOfWork.Object, currentUserService.Object, mapper, doctorRepo.Object);

            // Act
            var result = await service.CreateDoctorVacationAsyn(request);

            // Assert
            Assert.True(result.IsSuccess);

            vacationRepo.Verify(repo => repo.AddDoctorVacationAsync(It.Is<DoctorVacation>(v =>
                v.Status == DoctorVacationStatusEnum.NotStarted &&
                v.DoctorId == expectedDoctorId)), Times.Once);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateDoctorVacationAsyn_WhenStartDateIsToday_ShouldSetStatusInProgress_AndSave()
        {
            // Arrange
            var vacationRepo = DoctorVacationMocks.DoctorVacationRepository();
            var queryService = DoctorVacationMocks.DoctorVacationQueryService();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var mapper = CommonMocks.Mapper();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int clinicId = 1;
            int userId = 100;
            int expectedDoctorId = 5;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(expectedDoctorId);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var request = DoctorVacationBuilder.CreateRequest(userId, today);

            vacationRepo.Setup(repo => repo.AddDoctorVacationAsync(It.IsAny<DoctorVacation>())).Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DoctorVacationService(vacationRepo.Object, queryService.Object, unitOfWork.Object, currentUserService.Object, mapper, doctorRepo.Object);

            // Act
            var result = await service.CreateDoctorVacationAsyn(request);

            // Assert
            Assert.True(result.IsSuccess);

            vacationRepo.Verify(repo => repo.AddDoctorVacationAsync(It.Is<DoctorVacation>(v =>
                v.Status == DoctorVacationStatusEnum.InProgress)), Times.Once);
        }

        [Fact]
        public async Task UpdateDoctorVacationAsyn_WhenVacationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var vacationRepo = DoctorVacationMocks.DoctorVacationRepository();
            var queryService = DoctorVacationMocks.DoctorVacationQueryService();
            var unitOfWork = CommonMocks.UnitOfWork();
            var currentUserService = CommonMocks.CurrentUserService();
            var mapper = CommonMocks.Mapper();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int clinicId = 1;
            int userId = 100;
            int expectedDoctorId = 5;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(expectedDoctorId);

            var request = DoctorVacationBuilder.CreateRequest(userId, DateOnly.FromDateTime(DateTime.UtcNow));

            vacationRepo.Setup(repo => repo.GetDoctorVacationByIdAsync(request.Id.Value, expectedDoctorId, clinicId, true))
                        .ReturnsAsync((DoctorVacation)null);

            var service = new DoctorVacationService(vacationRepo.Object, queryService.Object, unitOfWork.Object, currentUserService.Object, mapper, doctorRepo.Object);

            // Act
            var result = await service.UpdateDoctorVacationAsyn(request);

            // Assert
            Assert.False(result.IsSuccess);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
    }
}