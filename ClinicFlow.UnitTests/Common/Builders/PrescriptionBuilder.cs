using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class PrescriptionBuilder
    {
        public static CreatePrescriptionDtoRequest CreateRequest(
            int medicalRecordId = 1,
            List<CreatePrescriptionItemDtoRequest>? items = null)
        {
            return new CreatePrescriptionDtoRequest
            {
                MedicalRecordId = medicalRecordId,
                Notes = "Take after meals",
                PrescriptionItems = items ?? new List<CreatePrescriptionItemDtoRequest>
                {
                    new CreatePrescriptionItemDtoRequest
                    {
                        MedicationName = "Amoxicillin",
                        Dosage = "500mg",
                        Frequency = "3 times daily",
                        Duration = "7 days",
                        Instructions = "After food"
                    }
                }
            };
        }

        public static UpdatePrescriptionDtoRequest UpdateRequest(
            int id = 1,
            List<UpdatePrescriptionItemDtoRequest>? items = null)
        {
            return new UpdatePrescriptionDtoRequest
            {
                Id = id,
                Notes = "Updated notes",
                PrescriptionItems = items ?? new List<UpdatePrescriptionItemDtoRequest>
                {
                    new UpdatePrescriptionItemDtoRequest
                    {
                        Id = 1,
                        MedicationName = "Amoxicillin",
                        Dosage = "1000mg",
                        Frequency = "Twice daily",
                        Duration = "10 days",
                        Instructions = "With plenty of water"
                    }
                }
            };
        }

        public static Prescription CreatePrescription(
            int id = 1,
            int medicalRecordId = 1,
            int doctorId = 20,
            List<PrescriptionItem>? items = null)
        {
            return new Prescription
            {
                Id = id,
                MedicalRecordId = medicalRecordId,
                DoctorId = doctorId,
                IssuedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                Notes = "Initial prescription notes",
                PrescriptionItems = items ?? new List<PrescriptionItem>
                {
                    new PrescriptionItem
                    {
                        Id = 1,
                        PrescriptionId = id,
                        MedicationName = "Amoxicillin",
                        Dosage = "500mg",
                        Frequency = "3 times daily",
                        Duration = "7 days",
                        Instructions = "After food"
                    }
                }
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
