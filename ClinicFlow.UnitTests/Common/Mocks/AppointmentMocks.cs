using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public class AppointmentMocks
    {

        public static Mock<IAppointmentRepository> AppointmentRepository() => new Mock<IAppointmentRepository>();
        public static Mock<IPatientRepository> PatientRepository() => new Mock<IPatientRepository>();
        public static Mock<IDoctorRepository> DoctorRepository() => new Mock<IDoctorRepository>();
        public static Mock<IClinicRepository> ClinicRepository() => new Mock<IClinicRepository>();
        public static Mock<IDoctorScheduleRepository> DoctorScheduleRepository() => new Mock<IDoctorScheduleRepository>();
        public static Mock<IDoctorVacationRepository> DoctorVacationRepository() => new Mock<IDoctorVacationRepository>();
        public static Mock<IClinicWorkingHourRepository> ClinicWorkingHourRepository() => new Mock<IClinicWorkingHourRepository>();
        public static Mock<IInvoiceRepository> InvoiceRepository() => new Mock<IInvoiceRepository>();
        public static Mock<IPaymentRepository> PaymentRepository() => new Mock<IPaymentRepository>();
        public static Mock<IAppointmentQueryService> AppointmentQueryService() => new Mock<IAppointmentQueryService>();

        public static Mock<IClinicWorkingHoursService> ClinicWorkingHoursService() => new Mock<IClinicWorkingHoursService>();
        public static Mock<IDoctorScheduleService> DoctorScheduleService() => new Mock<IDoctorScheduleService>();
        public static Mock<IDoctorVacationService> DoctorVacationService() => new Mock<IDoctorVacationService>();

    }
}
