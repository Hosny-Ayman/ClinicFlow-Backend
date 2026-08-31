using ClinicFlow.Domain.Interfaces;
using Moq;

namespace ClinicFlow.UnitTests.Common.Mocks
{
    public class MedicalRecordMocks
    {
        public static Mock<IMedicalRecordRepository> MedicalRecordRepository() => new Mock<IMedicalRecordRepository>();
        public static Mock<IAppointmentRepository> AppointmentRepository() => new Mock<IAppointmentRepository>();
        public static Mock<IDoctorRepository> DoctorRepository() => new Mock<IDoctorRepository>();
    }
}
