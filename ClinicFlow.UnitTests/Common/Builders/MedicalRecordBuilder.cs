using ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class MedicalRecordBuilder
    {
        public static CreateMedicalRecordDtoRequest CreateRequest(int appointmentId = 1)
        {
            return new CreateMedicalRecordDtoRequest
            {
                AppointmentId = appointmentId,
                Diagnosis = "Acute Bronchitis",
                Symptoms = "Cough, Fever, Fatigue",
                TreatmentPlan = "Rest and Hydration",
                Notes = "Follow-up in 1 week"
            };
        }

        public static UpdateMedicalRecordDtoRequest UpdateRequest(int id = 1)
        {
            return new UpdateMedicalRecordDtoRequest
            {
                Id = id,
                Diagnosis = "Chronic Bronchitis",
                Symptoms = "Persistent Cough",
                TreatmentPlan = "Inhaler therapy",
                Notes = "Follow-up in 2 weeks"
            };
        }

        public static MedicalRecord CreateMedicalRecord(
            int id = 1,
            int appointmentId = 1,
            int patientId = 10,
            int doctorId = 20)
        {
            return new MedicalRecord
            {
                Id = id,
                AppointmentId = appointmentId,
                PatientId = patientId,
                DoctorId = doctorId,
                Diagnosis = "Acute Bronchitis",
                Symptoms = "Cough, Fever",
                TreatmentPlan = "Rest and Hydration",
                Notes = "Initial Visit",
                CreatedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            };
        }

        public static Appointment CreateAppointment(
            int id = 1,
            int clinicId = 10,
            int patientId = 10,
            int doctorId = 20,
            AppointmentStatusEnum status = AppointmentStatusEnum.InProgress)
        {
            return new Appointment
            {
                Id = id,
                ClinicId = clinicId,
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentDate = new DateOnly(2026, 8, 30),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(10, 30),
                Status = status
            };
        }
    }
}
