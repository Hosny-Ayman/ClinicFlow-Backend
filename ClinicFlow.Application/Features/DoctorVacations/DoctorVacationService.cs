using AutoMapper;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.DoctorVacations.DTOs;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;

namespace ClinicFlow.Application.Features.DoctorVacations
{
    public class DoctorVacationService
    {

        private readonly IDoctorVacationRepository _doctorVacationRepository;
        private readonly IDoctorVacationQueryService _doctorVacationQueryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorVacationService(IDoctorVacationRepository doctorVacationRepository, IDoctorVacationQueryService doctorVacationQueryService,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IDoctorRepository doctorRepository)
        {
            _doctorVacationRepository = doctorVacationRepository;
            _doctorVacationQueryService = doctorVacationQueryService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<int>> CreateDoctorVacationAsyn(Get_Create_Update_DoctorVacationDto request)
        {

            var doctorId = await _doctorRepository.GetDoctorIdByUserId(request.UserId, _currentUserService.ClinicId!.Value);

            if(doctorId == null)
            {
                return OperationResult<int>.NotFound();
            }

            var data = _mapper.Map<DoctorVacation>(request);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (request.StartDate > today)
                data.Status = DoctorVacationStatusEnum.NotStarted;
            else
                data.Status = DoctorVacationStatusEnum.InProgress;

            data.DoctorId = doctorId.Value;

            await _doctorVacationRepository.AddDoctorVacationAsync(data);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(data.Id);
        }


        public async Task<OperationResult<bool>> UpdateDoctorVacationAsyn(Get_Create_Update_DoctorVacationDto request)
        {

            var doctorId = await _doctorRepository.GetDoctorIdByUserId(request.UserId, _currentUserService.ClinicId!.Value);

            if (doctorId == null)
            {
                return OperationResult<bool>.NotFound();
            }

            var doctorVacation = await _doctorVacationRepository.GetDoctorVacationByIdAsync(request.Id!.Value, doctorId.Value, _currentUserService.ClinicId!.Value, true);

            if(doctorVacation == null)
            {
                return OperationResult<bool>.NotFound();
            }

            _mapper.Map(request, doctorVacation);       

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }

        public async Task<OperationResult<Get_Create_Update_DoctorVacationDto>> GetDoctorVacationInformationAsync(int userId,int vacationId)
        {

            var doctorId = await _doctorRepository.GetDoctorIdByUserId(userId, _currentUserService.ClinicId!.Value);

            if (doctorId == null)
            {
                return OperationResult<Get_Create_Update_DoctorVacationDto>.NotFound();
            }

            var data = await _doctorVacationRepository.GetDoctorVacationByIdAsync(vacationId, doctorId.Value, _currentUserService.ClinicId!.Value);

            if (data == null)
            {
                return OperationResult<Get_Create_Update_DoctorVacationDto>.NotFound();
            }

            var respons = _mapper.Map<Get_Create_Update_DoctorVacationDto>(data);

            respons.UserId = userId;

            return OperationResult<Get_Create_Update_DoctorVacationDto>.Success(respons);
        }

        public async Task<OperationResult<PagedResponse<GetAllDoctorVacationInformationDtoResponse>>> GetAllDoctorVacationInformationAsync(DoctorVacationSearchDtoRequest request)
        {

            var respons = await _doctorVacationQueryService.GetAllDoctorVacationInformationAsync(request, _currentUserService.ClinicId!.Value);

            return OperationResult<PagedResponse<GetAllDoctorVacationInformationDtoResponse>>.Success(respons);
        }


        public async Task<OperationResult<GetDoctorVacationDashboardInformationDtoResponse>> GetDoctorVacationDashboardInformationAsync()
        {
            var respons = await _doctorVacationQueryService.GetDoctorVacationDashboardInformationAsync(_currentUserService.ClinicId!.Value);

            return OperationResult<GetDoctorVacationDashboardInformationDtoResponse>.Success(respons);
        }

    }
}
