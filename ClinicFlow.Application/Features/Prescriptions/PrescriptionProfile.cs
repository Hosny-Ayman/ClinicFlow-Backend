using AutoMapper;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Prescriptions
{
    public class PrescriptionProfile : Profile
    {
        public PrescriptionProfile()
        {
            CreateMap<CreatePrescriptionDtoRequest, Prescription>();
            CreateMap<CreatePrescriptionItemDtoRequest, PrescriptionItem>();

            CreateMap<UpdatePrescriptionDtoRequest, Prescription>();
            CreateMap<UpdatePrescriptionItemDtoRequest, PrescriptionItem>();

            CreateMap<Prescription, GetPrescriptionDtoResponse>();
            CreateMap<PrescriptionItem, GetPrescriptionItemDtoResponse>();
        }
    }
}
