using ClinicFlow.Application.Common.Security;
using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public class PrescriptionMocks
    {
        public static Mock<IPrescriptionRepository> PrescriptionRepository() => new Mock<IPrescriptionRepository>();
        public static Mock<IMedicalRecordRepository> MedicalRecordRepository() => new Mock<IMedicalRecordRepository>();
        public static Mock<IAppointmentRepository> AppointmentRepository() => new Mock<IAppointmentRepository>();
        public static Mock<IUserRepository> UserRepository() => new Mock<IUserRepository>();
        public static Mock<ICheckService> CheckService() => new Mock<ICheckService>();
    }
}
