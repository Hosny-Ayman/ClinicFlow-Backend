using AutoMapper;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Specialties.DTOs.Requests;
using ClinicFlow.Domain.Interfaces;

namespace ClinicFlow.Application.Features.Specialties
{
    public class SpecialityService
    {

        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IMapper _mapper;

        public SpecialityService(ISpecialtyRepository specialtyRepository, IMapper mapper)
        {
            _specialtyRepository = specialtyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<GetAllSpecialityDtoRequest>>> GetAllSpecialityAsync()
        {
            var Specialites = await _specialtyRepository.getAllSpecialtiesAsync();

            var SpecialitesDtos = _mapper.Map<List<GetAllSpecialityDtoRequest>>(Specialites);

            return OperationResult<List<GetAllSpecialityDtoRequest>>.Success(SpecialitesDtos);
        }


    }
}
