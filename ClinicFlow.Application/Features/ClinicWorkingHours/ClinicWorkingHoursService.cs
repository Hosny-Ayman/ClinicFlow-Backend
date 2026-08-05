using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.ClinicWorkingHours
{
    public class ClinicWorkingHoursService
    {

        private readonly IClinicWorkingHourRepository _clinicWorkingHourRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ClinicWorkingHoursService(IClinicWorkingHourRepository clinicWorkingHourRepository,
            IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _clinicWorkingHourRepository = clinicWorkingHourRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<bool>> CreateWorkingHoursAndDaysAsync(List<CreateClinicWorkingHourDtoRequest> request)
        {

            var days = _mapper.Map<List<ClinicWorkingHour>>(request);

            foreach (var day in days)
            {
                day.ClinicId = _currentUserService.ClinicId!.Value;
            }

            await _clinicWorkingHourRepository.AddWorkingHoursAndDaysAsync(days);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);

        }

        public async Task<OperationResult<List<GetAllWorkingHoursAndDaysDtoResponse?>>> GetAllWorkingHoursAndDaysAsync()
        {
            var WHD = await _clinicWorkingHourRepository.GetAllWorkingHoursAndDaysAsync(_currentUserService.ClinicId!.Value);
            List<GetAllWorkingHoursAndDaysDtoResponse> WHDDto = new List<GetAllWorkingHoursAndDaysDtoResponse>();

            if (WHD != null && WHD.Any())
            {
                WHDDto = _mapper.Map<List<GetAllWorkingHoursAndDaysDtoResponse>>(WHD);
            }

            

            return OperationResult<List<GetAllWorkingHoursAndDaysDtoResponse?>>.Success(WHDDto);


        }


        public async Task<OperationResult<bool>> UpdateWorkingHoursAndDaysAsync(List<UpdateClinicWorkingHoursAndDaysDtoRequest> request)
        {
            var workingHours = await _clinicWorkingHourRepository.GetAllWorkingHoursAndDaysAsync(_currentUserService.ClinicId!.Value, true);

            if (workingHours.Count == 0)
            {
                return OperationResult<bool>.NotFound();
            }

            var workingHoursToAdd = new List<ClinicWorkingHour>();

            var workingHoursDictionary = workingHours.ToDictionary(x => x.Id);

            foreach (var dto in request)
            {
               
                if (dto.Id == 0)
                {
                    var entity = _mapper.Map<ClinicWorkingHour>(dto);

                    entity.ClinicId = _currentUserService.ClinicId!.Value;

                    workingHoursToAdd.Add(entity);

                    continue;
                }

                
                if (!workingHoursDictionary.TryGetValue(dto.Id, out var entityToUpdate))
                {
                    return OperationResult<bool>.BadRequest();
                }

                _mapper.Map(dto, entityToUpdate);
            }

            if (workingHoursToAdd.Any())
            {
                await _clinicWorkingHourRepository.AddWorkingHoursAndDaysAsync(workingHoursToAdd);
            }

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }
    }
}
