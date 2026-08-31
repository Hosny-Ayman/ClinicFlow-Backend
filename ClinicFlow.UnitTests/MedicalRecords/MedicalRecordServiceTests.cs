using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.MedicalRecords;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

namespace ClinicFlow.UnitTests.MedicalRecords
{
    public class MedicalRecordServiceTests
    {
        private MedicalRecordService CreateService(
            Mock<IMedicalRecordRepository>? medicalRecordRepo = null,
            Mock<IAppointmentRepository>? appointmentRepo = null,
            Mock<IDoctorRepository>? doctorRepo = null,
            Mock<IUnitOfWork>? uow = null,
            Mock<ICurrentUserService>? currentUser = null)
        {
            var mapper = CommonMocks.Mapper();
            var logger = CommonMocks.Logger<MedicalRecordService>();

            return new MedicalRecordService(
                medicalRecordRepo?.Object ?? MedicalRecordMocks.MedicalRecordRepository().Object,
                appointmentRepo?.Object ?? MedicalRecordMocks.AppointmentRepository().Object,
                doctorRepo?.Object ?? MedicalRecordMocks.DoctorRepository().Object,
                uow?.Object ?? CommonMocks.UnitOfWork().Object,
                mapper,
                currentUser?.Object ?? CommonMocks.CurrentUserService().Object,
                logger.Object
            );
        }

        #region CREATE TESTS

        [Fact]
        public async Task CreateMedicalRecordAsync_WhenValidAndInProgress_ShouldAddAndSave()
        {
            // Arrange
            int clinicId = 10;
            var request = MedicalRecordBuilder.CreateRequest(appointmentId: 1);
            var appointment = MedicalRecordBuilder.CreateAppointment(id: 1, clinicId: clinicId, status: AppointmentStatusEnum.InProgress);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appointmentRepo = MedicalRecordMocks.AppointmentRepository();
            appointmentRepo.Setup(r => r.GetAppointmentByIdAsync(1, clinicId, false)).ReturnsAsync(appointment);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.HasMedicalRecordForAppointmentAsync(1)).ReturnsAsync(false);
            medicalRecordRepo.Setup(r => r.AddMedicalRecordAsync(It.IsAny<MedicalRecord>())).Returns(Task.CompletedTask);

            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                appointmentRepo: appointmentRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.CreateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(OperationStatus.Success, result.Status);
            medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.Is<MedicalRecord>(m =>
                m.AppointmentId == 1 &&
                m.PatientId == appointment.PatientId &&
                m.DoctorId == appointment.DoctorId &&
                m.Diagnosis == request.Diagnosis
            )), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateMedicalRecordAsync_WhenAppointmentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int clinicId = 10;
            var request = MedicalRecordBuilder.CreateRequest(appointmentId: 99);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appointmentRepo = MedicalRecordMocks.AppointmentRepository();
            appointmentRepo.Setup(r => r.GetAppointmentByIdAsync(99, clinicId, false)).ReturnsAsync((Appointment?)null);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            var uow = CommonMocks.UnitOfWork();

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                appointmentRepo: appointmentRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.CreateMedicalRecordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.IsAny<MedicalRecord>()), Times.Never);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData(AppointmentStatusEnum.Scheduled)]
        [InlineData(AppointmentStatusEnum.CheckedIn)]
        [InlineData(AppointmentStatusEnum.Completed)]
        [InlineData(AppointmentStatusEnum.Cancelled)]
        [InlineData(AppointmentStatusEnum.NoShow)]
        public async Task CreateMedicalRecordAsync_WhenAppointmentStatusNotAllowed_ShouldReturnBadRequest(AppointmentStatusEnum status)
        {
            // Arrange
            int clinicId = 10;
            var request = MedicalRecordBuilder.CreateRequest(appointmentId: 1);
            var appointment = MedicalRecordBuilder.CreateAppointment(id: 1, clinicId: clinicId, status: status);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appointmentRepo = MedicalRecordMocks.AppointmentRepository();
            appointmentRepo.Setup(r => r.GetAppointmentByIdAsync(1, clinicId, false)).ReturnsAsync(appointment);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            var uow = CommonMocks.UnitOfWork();

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                appointmentRepo: appointmentRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.CreateMedicalRecordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.IsAny<MedicalRecord>()), Times.Never);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateMedicalRecordAsync_WhenMedicalRecordAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            int clinicId = 10;
            var request = MedicalRecordBuilder.CreateRequest(appointmentId: 1);
            var appointment = MedicalRecordBuilder.CreateAppointment(id: 1, clinicId: clinicId, status: AppointmentStatusEnum.InProgress);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appointmentRepo = MedicalRecordMocks.AppointmentRepository();
            appointmentRepo.Setup(r => r.GetAppointmentByIdAsync(1, clinicId, false)).ReturnsAsync(appointment);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.HasMedicalRecordForAppointmentAsync(1)).ReturnsAsync(true);

            var uow = CommonMocks.UnitOfWork();

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                appointmentRepo: appointmentRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.CreateMedicalRecordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.IsAny<MedicalRecord>()), Times.Never);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateMedicalRecordAsync_ShouldDerivePatientIdAndDoctorIdFromAppointment()
        {
            // Arrange
            int clinicId = 10;
            int expectedPatientId = 77;
            int expectedDoctorId = 88;

            var request = MedicalRecordBuilder.CreateRequest(appointmentId: 5);
            var appointment = MedicalRecordBuilder.CreateAppointment(
                id: 5,
                clinicId: clinicId,
                patientId: expectedPatientId,
                doctorId: expectedDoctorId,
                status: AppointmentStatusEnum.InProgress);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appointmentRepo = MedicalRecordMocks.AppointmentRepository();
            appointmentRepo.Setup(r => r.GetAppointmentByIdAsync(5, clinicId, false)).ReturnsAsync(appointment);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.HasMedicalRecordForAppointmentAsync(5)).ReturnsAsync(false);
            medicalRecordRepo.Setup(r => r.AddMedicalRecordAsync(It.IsAny<MedicalRecord>())).Returns(Task.CompletedTask);

            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                appointmentRepo: appointmentRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.CreateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.Is<MedicalRecord>(m =>
                m.PatientId == expectedPatientId &&
                m.DoctorId == expectedDoctorId
            )), Times.Once);
        }

        #endregion

        #region UPDATE TESTS

        [Fact]
        public async Task UpdateMedicalRecordAsync_WhenRecordNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int clinicId = 10;
            var request = MedicalRecordBuilder.UpdateRequest(id: 99);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(99, clinicId, true)).ReturnsAsync((MedicalRecord?)null);

            var uow = CommonMocks.UnitOfWork();

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_WhenDoctorOwnsRecord_ShouldUpdateAndSave()
        {
            // Arrange
            int clinicId = 10;
            int userId = 5;
            int doctorId = 20;

            var request = MedicalRecordBuilder.UpdateRequest(id: 1);
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: 1, doctorId: doctorId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);
            currentUser.Setup(c => c.UserId).Returns(userId);
            currentUser.Setup(c => c.Roles).Returns(new List<string> { nameof(RoleEnum.Doctor) });

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(1, clinicId, true)).ReturnsAsync(record);

            var doctorRepo = MedicalRecordMocks.DoctorRepository();
            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(doctorId);

            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                doctorRepo: doctorRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(request.Diagnosis, record.Diagnosis);
            Assert.Equal(request.Symptoms, record.Symptoms);
            Assert.Equal(request.TreatmentPlan, record.TreatmentPlan);
            Assert.Equal(request.Notes, record.Notes);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_WhenDifferentDoctor_ShouldReturnForbidden()
        {
            // Arrange
            int clinicId = 10;
            int userId = 5;
            int callingDoctorId = 20;
            int recordOwnerDoctorId = 99;

            var request = MedicalRecordBuilder.UpdateRequest(id: 1);
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: 1, doctorId: recordOwnerDoctorId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);
            currentUser.Setup(c => c.UserId).Returns(userId);
            currentUser.Setup(c => c.Roles).Returns(new List<string> { nameof(RoleEnum.Doctor) });

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(1, clinicId, true)).ReturnsAsync((MedicalRecord?)null);

            var doctorRepo = MedicalRecordMocks.DoctorRepository();
            doctorRepo.Setup(d => d.GetDoctorIdByUserId(userId, clinicId)).ReturnsAsync(callingDoctorId);

            var uow = CommonMocks.UnitOfWork();

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                doctorRepo: doctorRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_WhenClinicOwner_ShouldUpdateAndSave()
        {
            // Arrange
            int clinicId = 10;
            int userId = 1;
            int recordOwnerDoctorId = 20;

            var request = MedicalRecordBuilder.UpdateRequest(id: 1);
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: 1, doctorId: recordOwnerDoctorId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);
            currentUser.Setup(c => c.UserId).Returns(userId);
            currentUser.Setup(c => c.Roles).Returns(new List<string> { nameof(RoleEnum.ClinicOwner) });

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(1, clinicId, true)).ReturnsAsync(record);

            var doctorRepo = MedicalRecordMocks.DoctorRepository();
            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                doctorRepo: doctorRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(request.Diagnosis, record.Diagnosis);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
            doctorRepo.Verify(d => d.GetDoctorIdByUserId(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_WhenSuperAdmin_ShouldUpdateAndSave()
        {
            // Arrange
            int clinicId = 10;
            int userId = 1;
            int recordOwnerDoctorId = 20;

            var request = MedicalRecordBuilder.UpdateRequest(id: 1);
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: 1, doctorId: recordOwnerDoctorId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);
            currentUser.Setup(c => c.UserId).Returns(userId);
            currentUser.Setup(c => c.Roles).Returns(new List<string> { nameof(RoleEnum.SuperAdmin) });

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(1, clinicId, true)).ReturnsAsync(record);

            var doctorRepo = MedicalRecordMocks.DoctorRepository();
            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                doctorRepo: doctorRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_ShouldPreserveImmutableFields()
        {
            // Arrange
            int clinicId = 10;
            int initialId = 1;
            int initialAppointmentId = 5;
            int initialPatientId = 10;
            int initialDoctorId = 20;
            DateTime initialCreatedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var request = MedicalRecordBuilder.UpdateRequest(id: initialId);
            var record = MedicalRecordBuilder.CreateMedicalRecord(
                id: initialId,
                appointmentId: initialAppointmentId,
                patientId: initialPatientId,
                doctorId: initialDoctorId);
            record.CreatedAt = initialCreatedAt;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);
            currentUser.Setup(c => c.UserId).Returns(1);
            currentUser.Setup(c => c.Roles).Returns(new List<string> { nameof(RoleEnum.ClinicOwner) });

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(initialId, clinicId, true)).ReturnsAsync(record);

            var uow = CommonMocks.UnitOfWork();
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                uow: uow,
                currentUser: currentUser
            );

            // Act
            var result = await service.UpdateMedicalRecordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(initialId, record.Id);
            Assert.Equal(initialAppointmentId, record.AppointmentId);
            Assert.Equal(initialPatientId, record.PatientId);
            Assert.Equal(initialDoctorId, record.DoctorId);
            Assert.Equal(initialCreatedAt, record.CreatedAt);
        }

        #endregion

        #region GET TESTS

        [Fact]
        public async Task GetMedicalRecordByIdAsync_WhenFound_ShouldReturnMappedResponse()
        {
            // Arrange
            int clinicId = 10;
            int recordId = 1;
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: recordId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(recordId, clinicId, false)).ReturnsAsync(record);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                currentUser: currentUser
            );

            // Act
            var result = await service.GetMedicalRecordByIdAsync(recordId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(recordId, result.Data.Id);
            Assert.Equal(record.AppointmentId, result.Data.AppointmentId);
            Assert.Equal(record.PatientId, result.Data.PatientId);
            Assert.Equal(record.DoctorId, result.Data.DoctorId);
            Assert.Equal(record.Diagnosis, result.Data.Diagnosis);
            Assert.Equal(record.Symptoms, result.Data.Symptoms);
            Assert.Equal(record.TreatmentPlan, result.Data.TreatmentPlan);
            Assert.Equal(record.Notes, result.Data.Notes);
            Assert.Equal(record.CreatedAt, result.Data.CreatedAt);
        }

        [Fact]
        public async Task GetMedicalRecordByIdAsync_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int clinicId = 10;
            int recordId = 99;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByIdAsync(recordId, clinicId, false)).ReturnsAsync((MedicalRecord?)null);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                currentUser: currentUser
            );

            // Act
            var result = await service.GetMedicalRecordByIdAsync(recordId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetMedicalRecordByAppointmentIdAsync_WhenFound_ShouldReturnMappedResponse()
        {
            // Arrange
            int clinicId = 10;
            int appointmentId = 5;
            var record = MedicalRecordBuilder.CreateMedicalRecord(id: 1, appointmentId: appointmentId);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByAppointmentIdAsync(appointmentId, clinicId, false)).ReturnsAsync(record);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                currentUser: currentUser
            );

            // Act
            var result = await service.GetMedicalRecordByAppointmentIdAsync(appointmentId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(appointmentId, result.Data.AppointmentId);
            Assert.Equal(record.Id, result.Data.Id);
            Assert.Equal(record.Diagnosis, result.Data.Diagnosis);
        }

        [Fact]
        public async Task GetMedicalRecordByAppointmentIdAsync_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int clinicId = 10;
            int appointmentId = 99;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var medicalRecordRepo = MedicalRecordMocks.MedicalRecordRepository();
            medicalRecordRepo.Setup(r => r.GetMedicalRecordByAppointmentIdAsync(appointmentId, clinicId, false)).ReturnsAsync((MedicalRecord?)null);

            var service = CreateService(
                medicalRecordRepo: medicalRecordRepo,
                currentUser: currentUser
            );

            // Act
            var result = await service.GetMedicalRecordByAppointmentIdAsync(appointmentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        #endregion
    }
}
