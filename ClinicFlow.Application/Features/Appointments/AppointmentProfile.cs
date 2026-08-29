using AutoMapper;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Appointments
{
    public class AppointmentProfile:Profile
    {

        public AppointmentProfile()
        {
            CreateMap<Appointment,CreateAndEditAppointmentDto>().ReverseMap();
        }

    }
}
