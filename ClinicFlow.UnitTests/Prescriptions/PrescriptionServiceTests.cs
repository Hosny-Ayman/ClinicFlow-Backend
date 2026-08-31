using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Prescriptions;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicFlow.UnitTests.Prescriptions
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<IPrescriptionRepository> _prescriptionRepositoryMock;
        private readonly Mock<IMedicalRecordRepository> _medicalRecordRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICheckService> _checkServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<PrescriptionService>> _loggerMock;
        private readonly IMapper _mapper;

        private const int ClinicId = 10;
        private const int CurrentUserId = 99;

        public PrescriptionServiceTests()
        {
            _prescriptionRepositoryMock = PrescriptionMocks.PrescriptionRepository();
            _medicalRecordRepositoryMock = PrescriptionMocks.MedicalRecordRepository();
            _appointmentRepositoryMock = PrescriptionMocks.AppointmentRepository();
            _userRepositoryMock = PrescriptionMocks.UserRepository();
            _checkServiceMock = PrescriptionMocks.CheckService();
            _unitOfWorkMock = CommonMocks.UnitOfWork();
            _currentUserServiceMock = CommonMocks.CurrentUserService();
            _loggerMock = CommonMocks.Logger<PrescriptionService>();

            _currentUserServiceMock.Setup(x => x.ClinicId).Returns(ClinicId);
            _currentUserServiceMock.Setup(x => x.UserId).Returns(CurrentUserId);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PrescriptionProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private PrescriptionService CreateService()
        {
            return new PrescriptionService(
                _prescriptionRepositoryMock.Object,
                _medicalRecordRepositoryMock.Object,
                _appointmentRepositoryMock.Object,
                _userRepositoryMock.Object,
                _checkServiceMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _currentUserServiceMock.Object,
                _loggerMock.Object);
        }

        #region CREATE PRESCRIPTION TESTS

        [Fact]
        public async Task CreatePrescriptionAsync_WhenValidInProgressAppointmentAndMedicalRecord_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5, doctorId: 20);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, doctorId: 20, status: AppointmentStatusEnum.InProgress);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            _prescriptionRepositoryMock
                .Setup(r => r.HasPrescriptionForMedicalRecordAsync(1))
                .ReturnsAsync(false);

            _prescriptionRepositoryMock
                .Setup(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()))
                .Callback<Prescription>(p => p.Id = 100)
                .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(100, result.Data);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_WhenMedicalRecordNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 999);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(999, ClinicId, false))
                .ReturnsAsync((MedicalRecord?)null);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_WhenAppointmentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 999);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(999, ClinicId, false))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData(AppointmentStatusEnum.Scheduled)]
        [InlineData(AppointmentStatusEnum.CheckedIn)]
        [InlineData(AppointmentStatusEnum.Completed)]
        [InlineData(AppointmentStatusEnum.Cancelled)]
        [InlineData(AppointmentStatusEnum.NoShow)]
        public async Task CreatePrescriptionAsync_WhenAppointmentStatusNotInProgress_ShouldReturnBadRequest(AppointmentStatusEnum status)
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, status: status);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_WhenPrescriptionAlreadyExistsForMedicalRecord_ShouldReturnConflict()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, status: AppointmentStatusEnum.InProgress);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            _prescriptionRepositoryMock
                .Setup(r => r.HasPrescriptionForMedicalRecordAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_WhenPrescriptionItemsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1, items: new List<CreatePrescriptionItemDtoRequest>());
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, status: AppointmentStatusEnum.InProgress);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            _prescriptionRepositoryMock
                .Setup(r => r.HasPrescriptionForMedicalRecordAsync(1))
                .ReturnsAsync(false);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            _prescriptionRepositoryMock.Verify(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_DoctorIdComesFromMedicalRecord()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5, doctorId: 77);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, status: AppointmentStatusEnum.InProgress);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            _prescriptionRepositoryMock
                .Setup(r => r.HasPrescriptionForMedicalRecordAsync(1))
                .ReturnsAsync(false);

            Prescription? capturedPrescription = null;
            _prescriptionRepositoryMock
                .Setup(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()))
                .Callback<Prescription>(p => capturedPrescription = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(capturedPrescription);
            Assert.Equal(77, capturedPrescription.DoctorId);
            Assert.Equal(1, capturedPrescription.MedicalRecordId);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_PrescriptionItemsAreCreatedFromIncomingDto()
        {
            // Arrange
            var service = CreateService();
            var items = new List<CreatePrescriptionItemDtoRequest>
            {
                new CreatePrescriptionItemDtoRequest
                {
                    MedicationName = "Panadol",
                    Dosage = "500mg",
                    Frequency = "Twice daily",
                    Duration = "3 days",
                    Instructions = "After lunch"
                },
                new CreatePrescriptionItemDtoRequest
                {
                    MedicationName = "Cataflam",
                    Dosage = "50mg",
                    Frequency = "Once daily",
                    Duration = "5 days",
                    Instructions = "Before bedtime"
                }
            };
            var request = PrescriptionBuilder.CreateRequest(medicalRecordId: 1, items: items);
            var medicalRecord = PrescriptionBuilder.CreateMedicalRecord(id: 1, appointmentId: 5, doctorId: 20);
            var appointment = PrescriptionBuilder.CreateAppointment(id: 5, clinicId: ClinicId, status: AppointmentStatusEnum.InProgress);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetMedicalRecordByIdAsync(1, ClinicId, false))
                .ReturnsAsync(medicalRecord);

            _appointmentRepositoryMock
                .Setup(r => r.GetAppointmentByIdAsync(5, ClinicId, false))
                .ReturnsAsync(appointment);

            _prescriptionRepositoryMock
                .Setup(r => r.HasPrescriptionForMedicalRecordAsync(1))
                .ReturnsAsync(false);

            Prescription? capturedPrescription = null;
            _prescriptionRepositoryMock
                .Setup(r => r.AddPrescriptionAsync(It.IsAny<Prescription>()))
                .Callback<Prescription>(p => capturedPrescription = p)
                .Returns(Task.CompletedTask);

            // Act
            var result = await service.CreatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(capturedPrescription);
            Assert.Equal(2, capturedPrescription.PrescriptionItems.Count);
            Assert.Contains(capturedPrescription.PrescriptionItems, i => i.MedicationName == "Panadol" && i.Dosage == "500mg");
            Assert.Contains(capturedPrescription.PrescriptionItems, i => i.MedicationName == "Cataflam" && i.Dosage == "50mg");
        }

        #endregion

        #region GET PRESCRIPTION BY ID TESTS

        [Fact]
        public async Task GetPrescriptionByIdAsync_WhenPrescriptionExists_ShouldReturnPrescriptionWithItems()
        {
            // Arrange
            var service = CreateService();
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, medicalRecordId: 2, doctorId: 20);

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, false))
                .ReturnsAsync(prescription);

            // Act
            var result = await service.GetPrescriptionByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal(2, result.Data.MedicalRecordId);
            Assert.Equal(20, result.Data.DoctorId);
            Assert.Single(result.Data.PrescriptionItems);
            Assert.Equal("Amoxicillin", result.Data.PrescriptionItems[0].MedicationName);
        }

        [Fact]
        public async Task GetPrescriptionByIdAsync_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(999, ClinicId, false))
                .ReturnsAsync((Prescription?)null);

            // Act
            var result = await service.GetPrescriptionByIdAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        #endregion

        #region GET PRESCRIPTION BY MEDICAL RECORD ID TESTS

        [Fact]
        public async Task GetPrescriptionByMedicalRecordIdAsync_WhenPrescriptionExists_ShouldReturnPrescriptionWithItems()
        {
            // Arrange
            var service = CreateService();
            var prescription = PrescriptionBuilder.CreatePrescription(id: 10, medicalRecordId: 5, doctorId: 20);

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByMedicalRecordIdAsync(5, ClinicId, false))
                .ReturnsAsync(prescription);

            // Act
            var result = await service.GetPrescriptionByMedicalRecordIdAsync(5);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(10, result.Data.Id);
            Assert.Equal(5, result.Data.MedicalRecordId);
            Assert.Single(result.Data.PrescriptionItems);
        }

        [Fact]
        public async Task GetPrescriptionByMedicalRecordIdAsync_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByMedicalRecordIdAsync(999, ClinicId, false))
                .ReturnsAsync((Prescription?)null);

            // Act
            var result = await service.GetPrescriptionByMedicalRecordIdAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        #endregion

        #region UPDATE PRESCRIPTION & ITEM RECONCILIATION TESTS

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenOwningDoctor_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 1);
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20);
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenClinicOwnerOrAdmin_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 1);
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20);
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenSuperAdmin_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 1);
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20);
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenDifferentDoctor_ShouldReturnForbidden()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 1);
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20);
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(false);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Forbidden, result.Status);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenPrescriptionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 999);

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(999, ClinicId, true))
                .ReturnsAsync((Prescription?)null);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenPrescriptionItemsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var service = CreateService();
            var request = PrescriptionBuilder.UpdateRequest(id: 1, items: new List<UpdatePrescriptionItemDtoRequest>());
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20);
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenExistingItemMatchingId_ShouldUpdateItemFields()
        {
            // Arrange
            var service = CreateService();
            var existingItem = new PrescriptionItem
            {
                Id = 1,
                PrescriptionId = 1,
                MedicationName = "Old Med",
                Dosage = "100mg",
                Frequency = "Once",
                Duration = "3 days",
                Instructions = "None"
            };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20, items: new List<PrescriptionItem> { existingItem });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 1,
                Notes = "Updated",
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest
                    {
                        Id = 1,
                        MedicationName = "New Med",
                        Dosage = "500mg",
                        Frequency = "Twice",
                        Duration = "7 days",
                        Instructions = "After food"
                    }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Med", existingItem.MedicationName);
            Assert.Equal("500mg", existingItem.Dosage);
            Assert.Equal("Twice", existingItem.Frequency);
            Assert.Equal("7 days", existingItem.Duration);
            Assert.Equal("After food", existingItem.Instructions);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenItemIdZero_ShouldAddNewPrescriptionItem()
        {
            // Arrange
            var service = CreateService();
            var existingItem = new PrescriptionItem { Id = 1, PrescriptionId = 1, MedicationName = "Med 1", Dosage = "100mg", Frequency = "1x", Duration = "3d" };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20, items: new List<PrescriptionItem> { existingItem });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 1,
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 1, MedicationName = "Med 1", Dosage = "100mg", Frequency = "1x", Duration = "3d" },
                    new UpdatePrescriptionItemDtoRequest { Id = 0, MedicationName = "New Added Med", Dosage = "200mg", Frequency = "2x", Duration = "5d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, prescription.PrescriptionItems.Count);
            Assert.Contains(prescription.PrescriptionItems, i => i.MedicationName == "New Added Med");
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenExistingItemOmittedFromRequest_ShouldRemoveItem()
        {
            // Arrange
            var service = CreateService();
            var item1 = new PrescriptionItem { Id = 1, PrescriptionId = 1, MedicationName = "Keep Med", Dosage = "100mg", Frequency = "1x", Duration = "3d" };
            var item2 = new PrescriptionItem { Id = 2, PrescriptionId = 1, MedicationName = "Remove Med", Dosage = "50mg", Frequency = "1x", Duration = "3d" };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20, items: new List<PrescriptionItem> { item1, item2 });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 1,
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 1, MedicationName = "Keep Med", Dosage = "100mg", Frequency = "1x", Duration = "3d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(prescription.PrescriptionItems);
            Assert.DoesNotContain(prescription.PrescriptionItems, i => i.Id == 2);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenItemIdBelongsToAnotherPrescription_ShouldReturnBadRequest()
        {
            // Arrange
            var service = CreateService();
            var item1 = new PrescriptionItem { Id = 1, PrescriptionId = 1, MedicationName = "Med 1", Dosage = "100mg", Frequency = "1x", Duration = "3d" };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20, items: new List<PrescriptionItem> { item1 });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 1,
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 999, MedicationName = "Wrong Med", Dosage = "100mg", Frequency = "1x", Duration = "3d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_MixedUpdate_ShouldPersistAllChangesInOneSaveChanges()
        {
            // Arrange
            var service = CreateService();
            var itemToKeep = new PrescriptionItem { Id = 1, PrescriptionId = 1, MedicationName = "Keep", Dosage = "100mg", Frequency = "1x", Duration = "3d" };
            var itemToRemove = new PrescriptionItem { Id = 2, PrescriptionId = 1, MedicationName = "Remove", Dosage = "50mg", Frequency = "1x", Duration = "3d" };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 1, doctorId: 20, items: new List<PrescriptionItem> { itemToKeep, itemToRemove });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 1,
                Notes = "New Prescription Notes",
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 1, MedicationName = "Keep Updated", Dosage = "200mg", Frequency = "2x", Duration = "5d" },
                    new UpdatePrescriptionItemDtoRequest { Id = 0, MedicationName = "Brand New", Dosage = "300mg", Frequency = "3x", Duration = "7d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(1, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Prescription Notes", prescription.Notes);
            Assert.Equal(2, prescription.PrescriptionItems.Count);
            Assert.Equal("Keep Updated", itemToKeep.MedicationName);
            Assert.Contains(prescription.PrescriptionItems, i => i.MedicationName == "Brand New");
            Assert.DoesNotContain(prescription.PrescriptionItems, i => i.Id == 2);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_ImmutableParentFieldsPreserved()
        {
            // Arrange
            var service = CreateService();
            var originalIssuedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var prescription = new Prescription
            {
                Id = 10,
                MedicalRecordId = 5,
                DoctorId = 20,
                IssuedAt = originalIssuedAt,
                Notes = "Original Notes",
                PrescriptionItems = new List<PrescriptionItem>
                {
                    new PrescriptionItem { Id = 1, PrescriptionId = 10, MedicationName = "Med", Dosage = "100mg", Frequency = "1x", Duration = "3d" }
                }
            };
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 10,
                Notes = "Modified Notes",
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 1, MedicationName = "Med Updated", Dosage = "100mg", Frequency = "1x", Duration = "3d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(10, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, prescription.Id);
            Assert.Equal(5, prescription.MedicalRecordId);
            Assert.Equal(20, prescription.DoctorId);
            Assert.Equal(originalIssuedAt, prescription.IssuedAt);
            Assert.Equal("Modified Notes", prescription.Notes);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_PrescriptionItemCannotBeMovedToAnotherPrescription()
        {
            // Arrange
            var service = CreateService();
            var item = new PrescriptionItem { Id = 1, PrescriptionId = 10, MedicationName = "Med", Dosage = "100mg", Frequency = "1x", Duration = "3d" };
            var prescription = PrescriptionBuilder.CreatePrescription(id: 10, doctorId: 20, items: new List<PrescriptionItem> { item });
            var doctorUser = new User { Id = 50, ClinicId = ClinicId };

            var request = new UpdatePrescriptionDtoRequest
            {
                Id = 10,
                PrescriptionItems = new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest { Id = 1, MedicationName = "Med", Dosage = "200mg", Frequency = "2x", Duration = "4d" }
                }
            };

            _prescriptionRepositoryMock
                .Setup(r => r.GetPrescriptionByIdAsync(10, ClinicId, true))
                .ReturnsAsync(prescription);

            _userRepositoryMock
                .Setup(r => r.GetUserByDoctorIdAsync(20, ClinicId, false))
                .ReturnsAsync(doctorUser);

            _checkServiceMock
                .Setup(c => c.EnsureCanManageUser(50))
                .Returns(true);

            // Act
            var result = await service.UpdatePrescriptionAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, item.PrescriptionId);
        }

        #endregion
    }
}
