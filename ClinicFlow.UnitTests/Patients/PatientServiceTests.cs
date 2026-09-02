using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Patients;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicFlow.UnitTests.Patients
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<PatientService>> _loggerMock;
        private readonly Mock<IPatientQueryService> _queryServiceMock;
        private readonly IMapper _mapper;

        private const int ClinicId = 10;

        public PatientServiceTests()
        {
            _patientRepositoryMock = PatientMocks.PatientRepository();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _loggerMock = CommonMocks.Logger<PatientService>();
            _queryServiceMock = PatientMocks.PatientQueryService();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(PatientProfile).Assembly);
            });
            _mapper = config.CreateMapper();
        }

        private PatientService CreateService()
        {
            return new PatientService(
                _patientRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _currentUserServiceMock.Object,
                _loggerMock.Object,
                _queryServiceMock.Object
            );
        }

        #region CreatePatientAsync

        [Fact]
        public async Task CreatePatientAsync_WhenValidRequest_ShouldCreatePatientAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.CreatePatientDto();

            _patientRepositoryMock
                .Setup(x => x.AddPatientAsync(It.IsAny<Patient>()))
                .Callback<Patient>(p => p.Id = 99)
                .ReturnsAsync(99);

            // Act
            var result = await service.CreatePatientAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(99, result.Data);

            _patientRepositoryMock.Verify(x => x.AddPatientAsync(It.Is<Patient>(p => 
                p.Person.FirstName == request.FirstName &&
                p.Person.Email == request.Email &&
                p.ClinicPatients.Count == 1 &&
                p.ClinicPatients.First().ClinicId == ClinicId
            )), Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetPatientByIdAsync

        [Fact]
        public async Task GetPatientByIdAsync_WhenPatientNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            int patientId = 5;

            _patientRepositoryMock
                .Setup(x => x.GetPatientByIdAsync(patientId, ClinicId, false))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await service.GetPatientByIdAsync(patientId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPatientByIdAsync_WhenPatientExists_ShouldReturnMappedPatient()
        {
            // Arrange
            var service = CreateService();
            int patientId = 5;
            var patient = PatientBuilder.CreatePatientEntity(id: patientId, clinicId: ClinicId);

            _patientRepositoryMock
                .Setup(x => x.GetPatientByIdAsync(patientId, ClinicId, false))
                .ReturnsAsync(patient);

            // Act
            var result = await service.GetPatientByIdAsync(patientId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(patient.Person.FirstName, result.Data.FirstName);
            Assert.Equal(patient.Gender.ToString(), result.Data.Gender);
        }

        #endregion

        #region UpdatePatientAsync

        [Fact]
        public async Task UpdatePatientAsync_WhenPatientNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.UpdatePatientDto(id: 5);

            _patientRepositoryMock
                .Setup(x => x.GetPatientByIdAsync(request.Id, ClinicId, true))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await service.UpdatePatientAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePatientAsync_WhenPatientExists_ShouldUpdateAndSave()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.UpdatePatientDto(id: 5, firstName: "UpdatedName");
            var patient = PatientBuilder.CreatePatientEntity(id: 5, clinicId: ClinicId, firstName: "OldName");

            _patientRepositoryMock
                .Setup(x => x.GetPatientByIdAsync(request.Id, ClinicId, true))
                .ReturnsAsync(patient);

            // Act
            var result = await service.UpdatePatientAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("UpdatedName", patient.Person.FirstName);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetAllPatientsAsync

        [Fact]
        public async Task GetAllPatientsAsync_ShouldCallQueryService()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.CreateSearchRequest();
            var expectedResponse = new PagedResponse<GetAllPatientsDtoResponse>(
                new List<GetAllPatientsDtoResponse>(), 1, 10, 0
            );

            _queryServiceMock
                .Setup(x => x.GetAllPatientsAsync(request, ClinicId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetAllPatientsAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedResponse, result.Data);
            _queryServiceMock.Verify(x => x.GetAllPatientsAsync(request, ClinicId), Times.Once);
        }

        #endregion

        #region GetPatientInformationForAppointmentAsync

        [Fact]
        public async Task GetPatientInformationForAppointmentAsync_WhenResponseNull_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.CreateAppointmentSearchRequest();

            _queryServiceMock
                .Setup(x => x.GetPatientInformationForAppointmentAsync(request, ClinicId))
                .ReturnsAsync((GetPatientInformationForAppointmentDtoResponse?)null);

            // Act
            var result = await service.GetPatientInformationForAppointmentAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetPatientInformationForAppointmentAsync_WhenResponseNotNull_ShouldReturnSuccess()
        {
            // Arrange
            var service = CreateService();
            var request = PatientBuilder.CreateAppointmentSearchRequest();
            var expectedResponse = new GetPatientInformationForAppointmentDtoResponse
            {
                Id = 1,
                FullName = "Jane Doe",
                PhoneNumber = request.PhoneNumber
            };

            _queryServiceMock
                .Setup(x => x.GetPatientInformationForAppointmentAsync(request, ClinicId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetPatientInformationForAppointmentAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Same(expectedResponse, result.Data);
        }

        #endregion
    }
}
