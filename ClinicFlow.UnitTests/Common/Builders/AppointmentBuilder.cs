using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class AppointmentBuilder
    {
        public static CreateAndEditAppointmentDto CreateRequest(AppointmentStatusEnum status = AppointmentStatusEnum.Scheduled)
        {
            return new CreateAndEditAppointmentDto
            {
                DoctorId = 1,
                PatientId = 1,
                AppointmentDate = new DateOnly(2027, 1, 1),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(10, 30),
                Status = status,
                Notes = "Test Appointment"
            };
        }

        public static DoctorAvailableSlotsDtoRequest CreateSlotsRequest()
        {
            return new DoctorAvailableSlotsDtoRequest
            {
                doctorId = 1,
                appointmentDate = new DateOnly(2027, 1, 1)
            };
        }

        public static Appointment CreateAppointment(int id, AppointmentStatusEnum status = AppointmentStatusEnum.Scheduled)
        {
            return new Appointment
            {
                Id = id,
                DoctorId = 1,
                PatientId = 1,
                ClinicId = 10,
                AppointmentDate = new DateOnly(2027, 1, 1),
                StartTime = new TimeOnly(10, 0),
                Status = status
            };
        }

        public static Doctor CreateDoctor()
        {
            return new Doctor
            {
                Id = 1,
                ConsultationFee = 500m
            };
        }
    }
}