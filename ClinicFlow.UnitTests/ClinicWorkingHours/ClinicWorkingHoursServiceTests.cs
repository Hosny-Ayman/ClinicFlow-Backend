using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.ClinicWorkingHours
{
    public class ClinicWorkingHoursServiceTests
    {
       

        [Fact]
        public async Task CreateWorkingHoursAndDaysAsync_WhenCalled_ShouldSetClinicIdAndSave()
        {
            // Arrange
            var workingHourRepo = ClinicWorkingHoursMocks.ClinicWorkingHourRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();

            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            var request = ClinicWorkingHoursBuilder.CreateRequestList();

            workingHourRepo.Setup(repo => repo.AddWorkingHoursAndDaysAsync(It.IsAny<List<ClinicWorkingHour>>()))
                           .Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var service = new ClinicWorkingHoursService(workingHourRepo.Object, mapper, unitOfWork.Object, currentUserService.Object);

            // Act
            var result = await service.CreateWorkingHoursAndDaysAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            workingHourRepo.Verify(repo => repo.AddWorkingHoursAndDaysAsync(
                It.Is<List<ClinicWorkingHour>>(list => list.All(item => item.ClinicId == clinicId))), Times.Once);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        

        [Fact]
        public async Task GetAllWorkingHoursAndDaysAsync_WhenDataExists_ShouldReturnMappedList()
        {
            // Arrange
            var workingHourRepo = ClinicWorkingHoursMocks.ClinicWorkingHourRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();

            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            var existingEntities = ClinicWorkingHoursBuilder.CreateEntitiesList(clinicId);

            workingHourRepo.Setup(repo => repo.GetAllWorkingHoursAndDaysAsync(clinicId, false))
                           .ReturnsAsync(existingEntities);

            var service = new ClinicWorkingHoursService(workingHourRepo.Object, mapper, unitOfWork.Object, currentUserService.Object);

            // Act
            var result = await service.GetAllWorkingHoursAndDaysAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(existingEntities.Count, result.Data.Count);
        }


        [Fact]
        public async Task UpdateWorkingHoursAndDaysAsync_WhenNoExistingHoursFound_ShouldReturnNotFound()
        {
            // Arrange
            var workingHourRepo = ClinicWorkingHoursMocks.ClinicWorkingHourRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();

            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            workingHourRepo.Setup(repo => repo.GetAllWorkingHoursAndDaysAsync(clinicId, true))
                           .ReturnsAsync(new List<ClinicWorkingHour>());

            var request = new List<UpdateClinicWorkingHoursAndDaysDtoRequest>();

            var service = new ClinicWorkingHoursService(workingHourRepo.Object, mapper, unitOfWork.Object, currentUserService.Object);

            // Act
            var result = await service.UpdateWorkingHoursAndDaysAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateWorkingHoursAndDaysAsync_WhenEntityIdNotFoundInDb_ShouldReturnBadRequest()
        {
            // Arrange
            var workingHourRepo = ClinicWorkingHoursMocks.ClinicWorkingHourRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();

            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            var existingEntities = ClinicWorkingHoursBuilder.CreateEntitiesList(clinicId);
            workingHourRepo.Setup(repo => repo.GetAllWorkingHoursAndDaysAsync(clinicId, true))
                           .ReturnsAsync(existingEntities);

            var request = new List<UpdateClinicWorkingHoursAndDaysDtoRequest>
            {
                new UpdateClinicWorkingHoursAndDaysDtoRequest { Id = 999, Day = DayOfWeek.Tuesday, IsClosed = true }
            };

            var service = new ClinicWorkingHoursService(workingHourRepo.Object, mapper, unitOfWork.Object, currentUserService.Object);

            // Act
            var result = await service.UpdateWorkingHoursAndDaysAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateWorkingHoursAndDaysAsync_WhenValidMixedRequest_ShouldUpdateExistingAndAddNew()
        {
            // Arrange
            var workingHourRepo = ClinicWorkingHoursMocks.ClinicWorkingHourRepository();
            var unitOfWork = CommonMocks.UnitOfWork();
            var mapper = CommonMocks.Mapper();
            var currentUserService = CommonMocks.CurrentUserService();

            int clinicId = 10;
            currentUserService.Setup(c => c.ClinicId).Returns(clinicId);

            var existingEntities = ClinicWorkingHoursBuilder.CreateEntitiesList(clinicId);
            workingHourRepo.Setup(repo => repo.GetAllWorkingHoursAndDaysAsync(clinicId, true))
                           .ReturnsAsync(existingEntities);

            var request = new List<UpdateClinicWorkingHoursAndDaysDtoRequest>
            {
                new UpdateClinicWorkingHoursAndDaysDtoRequest
                {
                    Id = 1,
                    Day = DayOfWeek.Sunday,
                    OpenTime = new TimeOnly(10, 0),
                    CloseTime = new TimeOnly(18, 0),
                    IsClosed = false
                },
                new UpdateClinicWorkingHoursAndDaysDtoRequest
                {
                    Id = 0, 
                    Day = DayOfWeek.Wednesday,
                    OpenTime = new TimeOnly(8, 0),
                    CloseTime = new TimeOnly(14, 0),
                    IsClosed = true
                }
            };

            workingHourRepo.Setup(repo => repo.AddWorkingHoursAndDaysAsync(It.IsAny<List<ClinicWorkingHour>>()))
                           .Returns(Task.CompletedTask);
            unitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            var service = new ClinicWorkingHoursService(workingHourRepo.Object, mapper, unitOfWork.Object, currentUserService.Object);

            // Act
            var result = await service.UpdateWorkingHoursAndDaysAsync(request);

            // Assert
            Assert.True(result.IsSuccess);

            workingHourRepo.Verify(repo => repo.AddWorkingHoursAndDaysAsync(
                It.Is<List<ClinicWorkingHour>>(list => list.Count == 1 && list.First().ClinicId == clinicId)), Times.Once);

            unitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }
    }
}