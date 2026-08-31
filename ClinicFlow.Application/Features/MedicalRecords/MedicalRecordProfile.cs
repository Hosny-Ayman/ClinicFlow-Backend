using AutoMapper;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Responses;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.MedicalRecords
{
    public class MedicalRecordProfile : Profile
    {
        public MedicalRecordProfile()
        {
            CreateMap<CreateMedicalRecordDtoRequest, MedicalRecord>();
            CreateMap<UpdateMedicalRecordDtoRequest, MedicalRecord>();
            CreateMap<MedicalRecord, GetMedicalRecordDtoResponse>();
        }
    }
}
