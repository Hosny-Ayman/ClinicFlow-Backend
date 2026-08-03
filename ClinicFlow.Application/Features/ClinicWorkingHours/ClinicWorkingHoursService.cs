using AutoMapper;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.ClinicWorkingHours
{
    public class ClinicWorkingHoursService
    {

        private readonly IClinicWorkingHourRepository _clinicWorkingHourRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ClinicWorkingHoursService(IClinicWorkingHourRepository clinicWorkingHourRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _clinicWorkingHourRepository = clinicWorkingHourRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult<bool>> CreateWorkingHoursAndDaysAsync(List<CreateClinicWorkingHourDtoRequest> DaysDto)
        {

            var Days = _mapper.Map<List<ClinicWorkingHour>>(DaysDto);

            await _clinicWorkingHourRepository.AddWorkingHoursAndDaysAsync(Days);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);

        }
    }
}
