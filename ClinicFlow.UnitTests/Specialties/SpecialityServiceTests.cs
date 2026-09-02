using AutoMapper;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Specialties;
using ClinicFlow.Application.Features.Specialties.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.Specialties
{
    public class SpecialityServiceTests
    {
        private readonly Mock<ISpecialtyRepository> _specialtyRepositoryMock;
        private readonly IMapper _mapper;

        public SpecialityServiceTests()
        {
            _specialtyRepositoryMock = DoctorMocks.SpecialtyRepository();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(SpecialityService).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private SpecialityService CreateService()
        {
            return new SpecialityService(
                _specialtyRepositoryMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task GetAllSpecialityAsync_WhenRepositoryReturnsEmpty_ShouldReturnSuccessWithEmptyList()
        {
            // Arrange
            var service = CreateService();
            _specialtyRepositoryMock
                .Setup(x => x.getAllSpecialtiesAsync())
                .ReturnsAsync(new List<Specialty>());

            // Act
            var result = await service.GetAllSpecialityAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);

            _specialtyRepositoryMock.Verify(x => x.getAllSpecialtiesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllSpecialityAsync_WhenRepositoryReturnsData_ShouldReturnSuccessWithMappedList()
        {
            // Arrange
            var service = CreateService();
            var specialties = new List<Specialty>
            {
                SpecialityBuilder.CreateSpecialtyEntity(id: 1, name: "Cardiology"),
                SpecialityBuilder.CreateSpecialtyEntity(id: 2, name: "Neurology")
            };

            _specialtyRepositoryMock
                .Setup(x => x.getAllSpecialtiesAsync())
                .ReturnsAsync(specialties);

            // Act
            var result = await service.GetAllSpecialityAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            
            // Verify Mapping
            Assert.Equal(specialties[0].Id, result.Data[0].Id);
            Assert.Equal(specialties[0].Name, result.Data[0].Name);
            Assert.Equal(specialties[1].Id, result.Data[1].Id);
            Assert.Equal(specialties[1].Name, result.Data[1].Name);

            _specialtyRepositoryMock.Verify(x => x.getAllSpecialtiesAsync(), Times.Once);
        }
    }
}
