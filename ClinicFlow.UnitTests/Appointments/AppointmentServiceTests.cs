using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Appointments;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Responses;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.UnitTests.Appointments
{
    public class AppointmentServiceTests
    {

        private AppointmentService CreateService(
            Mock<IAppointmentRepository> appRepo = null,
            Mock<IUnitOfWork> uow = null,
            Mock<ICurrentUserService> currentUser = null,
            Mock<IPatientRepository> patientRepo = null,
            Mock<IDoctorRepository> doctorRepo = null,
            Mock<IClinicWorkingHoursService> workingHoursService = null,
            Mock<IDoctorVacationService> vacationService = null,
            Mock<IDoctorScheduleService> scheduleService = null,
            Mock<IInvoiceRepository> invoiceRepo = null,
            Mock<IPaymentRepository> paymentRepo = null,
            Mock<IDoctorScheduleRepository> docScheduleRepo = null,
            Mock<IClinicWorkingHourRepository> clinicHourRepo = null,
            Mock<IAppointmentQueryService> queryService = null
            )
        {
            var mapper = CommonMocks.Mapper();
            var clinicRepo = AppointmentMocks.ClinicRepository();
            var docVacationRepo = AppointmentMocks.DoctorVacationRepository();

            var logger = new Mock<ILogger<AppointmentService>>();

            return new AppointmentService(
                appRepo?.Object ?? AppointmentMocks.AppointmentRepository().Object,
                uow?.Object ?? CommonMocks.UnitOfWork().Object,
                mapper,
                currentUser?.Object ?? CommonMocks.CurrentUserService().Object,
                patientRepo?.Object ?? AppointmentMocks.PatientRepository().Object,
                doctorRepo?.Object ?? AppointmentMocks.DoctorRepository().Object,
                clinicRepo.Object,
                docScheduleRepo?.Object ?? AppointmentMocks.DoctorScheduleRepository().Object,
                docVacationRepo.Object,
                workingHoursService?.Object ?? AppointmentMocks.ClinicWorkingHoursService().Object,
                scheduleService?.Object ?? AppointmentMocks.DoctorScheduleService().Object,
                vacationService?.Object ?? AppointmentMocks.DoctorVacationService().Object,
                clinicHourRepo?.Object ?? AppointmentMocks.ClinicWorkingHourRepository().Object,
                invoiceRepo?.Object ?? AppointmentMocks.InvoiceRepository().Object,
                paymentRepo?.Object ?? AppointmentMocks.PaymentRepository().Object,
                queryService?.Object ?? AppointmentMocks.AppointmentQueryService().Object,
                logger.Object
            );
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenAllValidationsPass_ShouldAddAndSave()
        {
            // Arrange
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();
            var appRepo = AppointmentMocks.AppointmentRepository();
            var uow = CommonMocks.UnitOfWork();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(true);
            appRepo.Setup(a => a.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId)).ReturnsAsync(false);

            appRepo.Setup(a => a.AddAppointmentAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);
            uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService, scheduleService: scheduleService);

            // Act
            var result = await service.AddAppointmentAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            appRepo.Verify(a => a.AddAppointmentAsync(It.Is<Appointment>(app => app.ClinicId == clinicId)), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenStatusIsCheckedIn_ShouldCreateInvoiceAndPayment()
        {
            // Arrange
            var request = AppointmentBuilder.CreateRequest(AppointmentStatusEnum.CheckedIn);
            int clinicId = 10;
            var doctor = AppointmentBuilder.CreateDoctor();

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();
            var appRepo = AppointmentMocks.AppointmentRepository();
            var invoiceRepo = AppointmentMocks.InvoiceRepository();
            var paymentRepo = AppointmentMocks.PaymentRepository();
            var uow = CommonMocks.UnitOfWork();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            doctorRepo.Setup(d => d.GetDoctorByIdAsync(request.DoctorId, clinicId,false)).ReturnsAsync(doctor); 
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(true);
            appRepo.Setup(a => a.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId)).ReturnsAsync(false);

            var service = CreateService(appRepo, uow, currentUser, patientRepo, doctorRepo, workingHoursService, vacationService, scheduleService, invoiceRepo, paymentRepo);

            // Act
            var result = await service.AddAppointmentAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            invoiceRepo.Verify(i => i.AddInvoiceAsync(It.Is<Invoice>(inv => inv.TotalAmount == doctor.ConsultationFee)), Times.Once);
            paymentRepo.Verify(p => p.AddPaymentAsync(It.Is<Payment>(pay => pay.Amount == doctor.ConsultationFee)), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenPatientNotInClinic_ShouldReturnNotFound()
        {
            // Arrange
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var appRepo = AppointmentMocks.AppointmentRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);

            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(false);

            var service = CreateService(appRepo: appRepo, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo);

            // Act
            var result = await service.AddAppointmentAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            appRepo.Verify(a => a.AddAppointmentAsync(It.IsAny<Appointment>()), Times.Never);
        }


        [Fact]
        public async Task UpdateAppointmentStatusAsync_WhenStatusChangesToCheckedIn_ShouldCreateInvoiceAndPayment()
        {
            // Arrange
            int appointmentId = 5;
            int clinicId = 10;
            var appointment = AppointmentBuilder.CreateAppointment(appointmentId, AppointmentStatusEnum.Scheduled);
            var doctor = AppointmentBuilder.CreateDoctor();

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appRepo = AppointmentMocks.AppointmentRepository();
            var doctorRepo = AppointmentMocks.DoctorRepository();
            var invoiceRepo = AppointmentMocks.InvoiceRepository();
            var paymentRepo = AppointmentMocks.PaymentRepository();
            var uow = CommonMocks.UnitOfWork();

            appRepo.Setup(a => a.GetAppointmentByIdAsync(appointmentId, clinicId, true)).ReturnsAsync(appointment);
            doctorRepo.Setup(d => d.GetDoctorByIdAsync(appointment.DoctorId, clinicId,false)).ReturnsAsync(doctor);

            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, invoiceRepo: invoiceRepo, paymentRepo: paymentRepo);

            // Act
            var result = await service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.CheckedIn);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AppointmentStatusEnum.CheckedIn, appointment.Status);
            invoiceRepo.Verify(i => i.AddInvoiceAsync(It.IsAny<Invoice>()), Times.Once);
            paymentRepo.Verify(p => p.AddPaymentAsync(It.IsAny<Payment>()), Times.Once);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDoctorAvailableSlotsByDateAsync_WhenValid_ShouldCalculateSlotsCorrectly()
        {
            // Arrange
            var request = AppointmentBuilder.CreateSlotsRequest();
            int clinicId = 10;

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var docScheduleRepo = AppointmentMocks.DoctorScheduleRepository();
            var appRepo = AppointmentMocks.AppointmentRepository();
            var clinicHourRepo = AppointmentMocks.ClinicWorkingHourRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.doctorId, clinicId)).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.appointmentDate, request.doctorId)).ReturnsAsync(false);

            var schedule = new DoctorSchedule { StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 30) };
            docScheduleRepo.Setup(s => s.GetDoctorScheduleAsync(request.appointmentDate.DayOfWeek, request.doctorId, clinicId,false)).ReturnsAsync(schedule);

            var workingHours = new ClinicWorkingHour { AppointmentDurationInMinutes = 30 };
            clinicHourRepo.Setup(c => c.GetWorkingHoursAndDaysByDayOfWeekAsync(clinicId, request.appointmentDate.DayOfWeek,false)).ReturnsAsync(workingHours);

            var existingAppointments = new List<Appointment>
            {
                new Appointment { StartTime = new TimeOnly(9, 30), Status = AppointmentStatusEnum.Scheduled }
            };
            appRepo.Setup(a => a.GetAllAppointmentsAsync(request.appointmentDate, clinicId, request.doctorId)).ReturnsAsync(existingAppointments);

            var service = CreateService(appRepo: appRepo, currentUser: currentUser, doctorRepo: doctorRepo, vacationService: vacationService, docScheduleRepo: docScheduleRepo, clinicHourRepo: clinicHourRepo);

            // Act
            var result = await service.GetDoctorAvailableSlotsByDateAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            var slots = result.Data;

            Assert.Equal(3, slots.Count);

            Assert.Equal(new TimeOnly(9, 0), slots[0].StartTime);
            Assert.Equal(SlotStatus.Available, slots[0].Status);

            Assert.Equal(new TimeOnly(9, 30), slots[1].StartTime);
            Assert.Equal(SlotStatus.Booked, slots[1].Status);

            Assert.Equal(new TimeOnly(10, 0), slots[2].StartTime);
            Assert.Equal(SlotStatus.Available, slots[2].Status);
        }

        [Fact]
        public async Task GetAdminDashboardStatisticsAsync_ShouldReturnSuccessWithData()
        {
            // Arrange
            int clinicId = 10;
            var today = DateOnly.FromDateTime(DateTime.Now);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();
            var expectedResponse = new GetAdminDashboardStatisticsDtoResponse
            {
                TotalAppointments = 15,
                AttendedAppointments = 10,
                WaitingAppointments = 3,
                CancelledAppointments = 2,
                AppointmentsByStatus = new List<AppointmentStatusBreakdownDto>(),
                AppointmentsByTimePeriod = new List<AppointmentTimeBreakdownDto>(),
                TopDoctors = new List<TopDoctorDto>()
            };

            queryService.Setup(q => q.GetAdminDashboardStatisticsAsync(clinicId, today))
                        .ReturnsAsync(expectedResponse);

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            // Act
            var result = await service.GetAdminDashboardStatisticsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Data);
            queryService.Verify(q => q.GetAdminDashboardStatisticsAsync(clinicId, today), Times.Once);
        }

        [Fact]
        public async Task GetAdminDashboardStatisticsAsync_ShouldCallQueryServiceWithCorrectParameters()
        {
            // Arrange
            int clinicId = 5;
            var today = DateOnly.FromDateTime(DateTime.Now);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();
            queryService.Setup(q => q.GetAdminDashboardStatisticsAsync(It.IsAny<int>(), It.IsAny<DateOnly>()))
                        .ReturnsAsync(new GetAdminDashboardStatisticsDtoResponse());

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            // Act
            await service.GetAdminDashboardStatisticsAsync();

            // Assert
            queryService.Verify(q => q.GetAdminDashboardStatisticsAsync(clinicId, today), Times.Once);
        }

        // --- Added Missing Tests ---
        
        #region AddAppointmentAsync Missing Scenarios

        [Fact]
        public async Task AddAppointmentAsync_WhenDoctorNotInClinic_ShouldReturnNotFound()
        {
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(false);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(uow: uow, currentUser: currentUser, doctorRepo: doctorRepo);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenClinicClosed_ShouldReturnBadRequest()
        {
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(false);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenDoctorOnVacation_ShouldReturnBadRequest()
        {
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(true);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenDoctorScheduleUnavailable_ShouldReturnBadRequest()
        {
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(false);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService, scheduleService: scheduleService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenAppointmentReserved_ShouldReturnConflict()
        {
            var request = AppointmentBuilder.CreateRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();
            var appRepo = AppointmentMocks.AppointmentRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(true);
            appRepo.Setup(a => a.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId)).ReturnsAsync(true);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService, scheduleService: scheduleService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.Conflict, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenDateInPast_ShouldReturnBadRequest()
        {
            var request = AppointmentBuilder.CreateRequest();
            request = request with { AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) };
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();
            var appRepo = AppointmentMocks.AppointmentRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(true);
            appRepo.Setup(a => a.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId)).ReturnsAsync(false);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService, scheduleService: scheduleService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddAppointmentAsync_WhenCheckedInButDoctorNotFound_ShouldReturnNotFound()
        {
            var request = AppointmentBuilder.CreateRequest(AppointmentStatusEnum.CheckedIn);
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var patientRepo = AppointmentMocks.PatientRepository();
            var workingHoursService = AppointmentMocks.ClinicWorkingHoursService();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var scheduleService = AppointmentMocks.DoctorScheduleService();
            var appRepo = AppointmentMocks.AppointmentRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.DoctorId, clinicId)).ReturnsAsync(true);
            doctorRepo.Setup(d => d.GetDoctorByIdAsync(request.DoctorId, clinicId, false)).ReturnsAsync((Doctor?)null); 
            patientRepo.Setup(p => p.IsPatientInClinicAsync(request.PatientId, clinicId)).ReturnsAsync(true);
            workingHoursService.Setup(w => w.IsTheClinicOpenAtThisAppointmentInsideProject(It.IsAny<Bookappointment>())).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId)).ReturnsAsync(false);
            scheduleService.Setup(s => s.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime)).ReturnsAsync(true);
            appRepo.Setup(a => a.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId)).ReturnsAsync(false);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo, patientRepo: patientRepo, workingHoursService: workingHoursService, vacationService: vacationService, scheduleService: scheduleService);

            var result = await service.AddAppointmentAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never); // Transaction fails, SaveChangesAsync not called
        }

        #endregion

        #region UpdateAppointmentStatusAsync Missing Scenarios

        [Fact]
        public async Task UpdateAppointmentStatusAsync_WhenAppointmentNotFound_ShouldReturnNotFound()
        {
            int appointmentId = 5;
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appRepo = AppointmentMocks.AppointmentRepository();
            appRepo.Setup(a => a.GetAppointmentByIdAsync(appointmentId, clinicId, true)).ReturnsAsync((Appointment?)null);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser);

            var result = await service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.CheckedIn);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAppointmentStatusAsync_WhenInvalidStatusTransition_ShouldReturnBadRequest()
        {
            int appointmentId = 5;
            int clinicId = 10;
            var appointment = AppointmentBuilder.CreateAppointment(appointmentId, AppointmentStatusEnum.Completed);
            
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appRepo = AppointmentMocks.AppointmentRepository();
            appRepo.Setup(a => a.GetAppointmentByIdAsync(appointmentId, clinicId, true)).ReturnsAsync(appointment);

            var uow = CommonMocks.UnitOfWork();
            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser);

            // Cannot transition from Completed to InProgress
            var result = await service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.InProgress);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAppointmentStatusAsync_WhenValidTransitionNotCheckedIn_ShouldNotCreateInvoice()
        {
            int appointmentId = 5;
            int clinicId = 10;
            var appointment = AppointmentBuilder.CreateAppointment(appointmentId, AppointmentStatusEnum.Scheduled);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appRepo = AppointmentMocks.AppointmentRepository();
            var invoiceRepo = AppointmentMocks.InvoiceRepository();
            var paymentRepo = AppointmentMocks.PaymentRepository();
            var uow = CommonMocks.UnitOfWork();

            appRepo.Setup(a => a.GetAppointmentByIdAsync(appointmentId, clinicId, true)).ReturnsAsync(appointment);

            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, invoiceRepo: invoiceRepo, paymentRepo: paymentRepo);

            // Valid transition: Scheduled -> Cancelled
            var result = await service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.Cancelled);

            Assert.True(result.IsSuccess);
            Assert.Equal(AppointmentStatusEnum.Cancelled, appointment.Status);
            invoiceRepo.Verify(i => i.AddInvoiceAsync(It.IsAny<Invoice>()), Times.Never);
            paymentRepo.Verify(p => p.AddPaymentAsync(It.IsAny<Payment>()), Times.Never);
            uow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentStatusAsync_WhenCheckedInButDoctorNotFound_ShouldReturnNotFound()
        {
            int appointmentId = 5;
            int clinicId = 10;
            var appointment = AppointmentBuilder.CreateAppointment(appointmentId, AppointmentStatusEnum.Scheduled);

            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var appRepo = AppointmentMocks.AppointmentRepository();
            var doctorRepo = AppointmentMocks.DoctorRepository();
            var uow = CommonMocks.UnitOfWork();

            appRepo.Setup(a => a.GetAppointmentByIdAsync(appointmentId, clinicId, true)).ReturnsAsync(appointment);
            doctorRepo.Setup(d => d.GetDoctorByIdAsync(appointment.DoctorId, clinicId, false)).ReturnsAsync((Doctor?)null);

            var service = CreateService(appRepo: appRepo, uow: uow, currentUser: currentUser, doctorRepo: doctorRepo);

            var result = await service.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatusEnum.CheckedIn);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
            uow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region GetDoctorAvailableSlotsByDateAsync Missing Scenarios

        [Fact]
        public async Task GetDoctorAvailableSlotsByDateAsync_WhenDoctorNotInClinic_ShouldReturnNotFound()
        {
            var request = AppointmentBuilder.CreateSlotsRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.doctorId, clinicId)).ReturnsAsync(false);

            var service = CreateService(currentUser: currentUser, doctorRepo: doctorRepo);

            var result = await service.GetDoctorAvailableSlotsByDateAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetDoctorAvailableSlotsByDateAsync_WhenDoctorOnVacation_ShouldReturnBadRequest()
        {
            var request = AppointmentBuilder.CreateSlotsRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var vacationService = AppointmentMocks.DoctorVacationService();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.doctorId, clinicId)).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.appointmentDate, request.doctorId)).ReturnsAsync(true);

            var service = CreateService(currentUser: currentUser, doctorRepo: doctorRepo, vacationService: vacationService);

            var result = await service.GetDoctorAvailableSlotsByDateAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
        }

        [Fact]
        public async Task GetDoctorAvailableSlotsByDateAsync_WhenScheduleNotFound_ShouldReturnNotFound()
        {
            var request = AppointmentBuilder.CreateSlotsRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var docScheduleRepo = AppointmentMocks.DoctorScheduleRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.doctorId, clinicId)).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.appointmentDate, request.doctorId)).ReturnsAsync(false);
            docScheduleRepo.Setup(s => s.GetDoctorScheduleAsync(request.appointmentDate.DayOfWeek, request.doctorId, clinicId, false)).ReturnsAsync((DoctorSchedule?)null);

            var service = CreateService(currentUser: currentUser, doctorRepo: doctorRepo, vacationService: vacationService, docScheduleRepo: docScheduleRepo);

            var result = await service.GetDoctorAvailableSlotsByDateAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        [Fact]
        public async Task GetDoctorAvailableSlotsByDateAsync_WhenWorkingHoursNotFound_ShouldReturnNotFound()
        {
            var request = AppointmentBuilder.CreateSlotsRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var doctorRepo = AppointmentMocks.DoctorRepository();
            var vacationService = AppointmentMocks.DoctorVacationService();
            var docScheduleRepo = AppointmentMocks.DoctorScheduleRepository();
            var appRepo = AppointmentMocks.AppointmentRepository();
            var clinicHourRepo = AppointmentMocks.ClinicWorkingHourRepository();

            doctorRepo.Setup(d => d.IsDoctorBelongToClinic(request.doctorId, clinicId)).ReturnsAsync(true);
            vacationService.Setup(v => v.HasDoctorVacationOnDate(request.appointmentDate, request.doctorId)).ReturnsAsync(false);
            var schedule = new DoctorSchedule { StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 30) };
            docScheduleRepo.Setup(s => s.GetDoctorScheduleAsync(request.appointmentDate.DayOfWeek, request.doctorId, clinicId, false)).ReturnsAsync(schedule);
            
            appRepo.Setup(a => a.GetAllAppointmentsAsync(request.appointmentDate, clinicId, request.doctorId)).ReturnsAsync(new List<Appointment>());
            clinicHourRepo.Setup(c => c.GetWorkingHoursAndDaysByDayOfWeekAsync(clinicId, request.appointmentDate.DayOfWeek, false)).ReturnsAsync((ClinicWorkingHour?)null);

            var service = CreateService(appRepo: appRepo, currentUser: currentUser, doctorRepo: doctorRepo, vacationService: vacationService, docScheduleRepo: docScheduleRepo, clinicHourRepo: clinicHourRepo);

            var result = await service.GetDoctorAvailableSlotsByDateAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.NotFound, result.Status);
        }

        #endregion

        #region Dashboard and Read Methods Missing Scenarios

        [Fact]
        public async Task GetAllAppointmentAsync_ShouldCallQueryServiceAndReturnSuccess()
        {
            var request = new AppointmentSearchDtoRequest();
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();
            var expectedResponse = new PagedResponse<GetAllAppointmentDtoResponse>(new List<GetAllAppointmentDtoResponse>(), 0, 1, 10);
            queryService.Setup(q => q.GetAllAppointmentAsync(request, clinicId)).ReturnsAsync(expectedResponse);

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            var result = await service.GetAllAppointmentAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Data);
            queryService.Verify(q => q.GetAllAppointmentAsync(request, clinicId), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentDashboardAsync_ShouldCallQueryServiceAndReturnSuccess()
        {
            var date = new DateOnly(2027, 1, 1);
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();
            var expectedResponse = new GetAppointmentDashboardDtoResponse();
            queryService.Setup(q => q.GetAppointmentDashboardAsync(date, clinicId)).ReturnsAsync(expectedResponse);

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            var result = await service.GetAppointmentDashboardAsync(date);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Data);
            queryService.Verify(q => q.GetAppointmentDashboardAsync(date, clinicId), Times.Once);
        }

        [Fact]
        public async Task GetDoctorAppointmentDashboardAsync_WhenDateInPast_ShouldReturnBadRequest()
        {
            int doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)); // past date
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            var result = await service.GetDoctorAppointmentDashboardAsync(doctorId, date);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationStatus.BadRequest, result.Status);
            queryService.Verify(q => q.GetDoctorAppointmentDashboardAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetDoctorAppointmentDashboardAsync_WhenValid_ShouldCallQueryServiceAndReturnSuccess()
        {
            int doctorId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)); // future date
            int clinicId = 10;
            var currentUser = CommonMocks.CurrentUserService();
            currentUser.Setup(c => c.ClinicId).Returns(clinicId);

            var queryService = AppointmentMocks.AppointmentQueryService();
            var expectedResponse = new GetAppointmentDashboardDtoResponse();
            queryService.Setup(q => q.GetDoctorAppointmentDashboardAsync(doctorId, date, clinicId)).ReturnsAsync(expectedResponse);

            var service = CreateService(currentUser: currentUser, queryService: queryService);

            var result = await service.GetDoctorAppointmentDashboardAsync(doctorId, date);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Data);
            queryService.Verify(q => q.GetDoctorAppointmentDashboardAsync(doctorId, date, clinicId), Times.Once);
        }

        #endregion

    }
}




