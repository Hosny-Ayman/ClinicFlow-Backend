using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.Appointments;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using ClinicFlow.UnitTests.Common.Builders;
using ClinicFlow.UnitTests.Common.Mocks;
using Moq;

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
            Mock<IClinicWorkingHourRepository> clinicHourRepo = null)
        {
            var mapper = CommonMocks.Mapper();
            var clinicRepo = AppointmentMocks.ClinicRepository();
            var docVacationRepo = AppointmentMocks.DoctorVacationRepository();
            var queryService = AppointmentMocks.AppointmentQueryService();

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
                queryService.Object
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
            var appointment = AppointmentBuilder.CreateAppointment(appointmentId);
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

    }
}
