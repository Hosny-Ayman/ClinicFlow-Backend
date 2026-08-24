using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Domain.Entities;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.DoctorSchedules
{
    public class DoctorScheduleServiceTests
    {

        [Fact]
        public async Task AddDoctorSchedulesInsideProjectAsync_ShouldAddSevenDaysSchedule()
        {
            // Arrange
            var doctorScheduleRepo = DoctorScheduleMocks.DoctorScheduleRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            var doctor = DoctorScheduleBuilder.CreateDoctor();

            doctorScheduleRepo.Setup(repo => repo.AddDoctorSchedulesAsync(It.IsAny<List<DoctorSchedule>>()))
                              .Returns(Task.CompletedTask);

            var service = new DoctorScheduleService(doctorScheduleRepo.Object, unitOfWork.Object, mapper, currentUserService.Object, doctorRepo.Object);

            // Act
            await service.AddDoctorSchedulesInsideProjectAsync(doctor);

            // Assert
            doctorScheduleRepo.Verify(repo => repo.AddDoctorSchedulesAsync(It.Is<List<DoctorSchedule>>(schedules => schedules.Count == 7)), Times.Once);
        }

        [Fact]
        public async Task GetAllDoctorSchedulesAsync_WhenDoctorNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var doctorScheduleRepo = DoctorScheduleMocks.DoctorScheduleRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int userId = 1;
            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            doctorRepo.Setup(repo => repo.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync((int?)null);

            var service = new DoctorScheduleService(doctorScheduleRepo.Object, unitOfWork.Object, mapper, currentUserService.Object, doctorRepo.Object);

            // Act
            var result = await service.GetAllDoctorSchedulesAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllDoctorSchedulesAsync_WhenDoctorExists_ShouldReturnMappedSchedules()
        {
            // Arrange
            var doctorScheduleRepo = DoctorScheduleMocks.DoctorScheduleRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int userId = 1;
            int clinicId = 10;
            int expectedDoctorId = 5;

            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(repo => repo.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(expectedDoctorId);

            var doctor = DoctorScheduleBuilder.CreateDoctor();
            var schedules = DoctorScheduleBuilder.CreateDoctorSchedules(doctor);

            doctorScheduleRepo.Setup(repo => repo.GetAllDoctorSchedulesAsync(expectedDoctorId, clinicId, false))  
                              .ReturnsAsync(schedules);

            var service = new DoctorScheduleService(doctorScheduleRepo.Object, unitOfWork.Object, mapper, currentUserService.Object, doctorRepo.Object);

            // Act
            var result = await service.GetAllDoctorSchedulesAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(schedules.Count, result.Data.Count);
        }

       
        [Fact]
        public async Task UpdateSchedulesInsideProjectAsync_WhenDoctorNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var doctorScheduleRepo = DoctorScheduleMocks.DoctorScheduleRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int userId = 1;
            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(repo => repo.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync((int?)null);

            var request = DoctorScheduleBuilder.CreateUpdateRequests();

            var service = new DoctorScheduleService(doctorScheduleRepo.Object, unitOfWork.Object, mapper, currentUserService.Object, doctorRepo.Object);

            // Act
            var result = await service.UpdateSchedulesInsideProjectAsync(request, userId);

            // Assert
            Assert.False(result.IsSuccess);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateSchedulesInsideProjectAsync_WhenDoctorExists_ShouldUpdateAndSave()
        {
            // Arrange
            var doctorScheduleRepo = DoctorScheduleMocks.DoctorScheduleRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();
            var doctorRepo = DoctorScheduleMocks.DoctorRepository();

            int userId = 1;
            int clinicId = 10;
            int expectedDoctorId = 5;

            var request = DoctorScheduleBuilder.CreateUpdateRequests();
            var doctor = DoctorScheduleBuilder.CreateDoctor();
            var existingSchedules = DoctorScheduleBuilder.CreateDoctorSchedules(doctor);

            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);
            doctorRepo.Setup(repo => repo.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(expectedDoctorId);

            doctorScheduleRepo.Setup(repo => repo.GetAllDoctorSchedulesAsync(expectedDoctorId, clinicId, true))
                              .ReturnsAsync(existingSchedules);

            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var service = new DoctorScheduleService(doctorScheduleRepo.Object, unitOfWork.Object, mapper, currentUserService.Object, doctorRepo.Object);

            // Act
            var result = await service.UpdateSchedulesInsideProjectAsync(request, userId);

            // Assert
            Assert.True(result.IsSuccess);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

    }
}
