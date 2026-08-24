
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class DoctorScheduleBuilder
    {
        public static Doctor CreateDoctor()
        {
            return new Doctor
            {
                Id = 5,
                UserId = 1,
            };
        }

        public static List<DoctorSchedule> CreateDoctorSchedules(Doctor doctor)
        {
            return new List<DoctorSchedule>
            {
                new DoctorSchedule { Id = 1, DoctorId = doctor.Id, DayOfWeek = DayOfWeek.Sunday, IsAvailable = true },
                new DoctorSchedule { Id = 2, DoctorId = doctor.Id, DayOfWeek = DayOfWeek.Monday, IsAvailable = true }
            };
        }

        public static List<UpdateAndGetDoctorScheduleDtoRequest> CreateUpdateRequests()
        {
            return new List<UpdateAndGetDoctorScheduleDtoRequest>
            {
                new UpdateAndGetDoctorScheduleDtoRequest { DayOfWeek = DayOfWeek.Sunday, IsAvailable = false },
                new UpdateAndGetDoctorScheduleDtoRequest { DayOfWeek = DayOfWeek.Monday, IsAvailable = false }
            };
        }
    }
}