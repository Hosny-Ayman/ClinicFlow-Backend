using AutoMapper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Requests;
using ClinicFlow.Application.Features.ClinicSetups.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.ClinicSetups
{
    public class ClinicSetupService
    {
        private readonly IClinicSetupRepository _clinicSetupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClinicSetupQueryService _clinicSetupService;

        public ClinicSetupService(IClinicSetupRepository clinicSetupRepository, IUnitOfWork unitOfWork, IMapper mapper
            , ICurrentUserService currentUserService, IClinicSetupQueryService clinicSetupQueryService)
        {

            _clinicSetupRepository = clinicSetupRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _clinicSetupService = clinicSetupQueryService;

        }


        public async Task<OperationResult<int>> CreateClinicSetupAsync(CreateClinicSetupDtoRequest setupDto)
        {
            var setup = _mapper.Map<ClinicSetup>(setupDto);

            setup.ClinicId = _currentUserService.ClinicId!.Value;

            await _clinicSetupRepository.AddClinicSetupStatusAsync(setup);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(setup.Id);

        }

        public async Task<OperationResult<GetClinicSetupStatusDtoResponse>> GetClinicSetupStatusAsync()
        {
            var steup = await _clinicSetupService.GetClinicSetupStatusAsync(_currentUserService.ClinicId!.Value);

            if(steup == null)
            {
                return OperationResult<GetClinicSetupStatusDtoResponse>.Success(null);
            }

            var steupDto = _mapper.Map<GetClinicSetupStatusDtoResponse>(steup);

            return OperationResult<GetClinicSetupStatusDtoResponse>.Success(steupDto);
        }



    }
}
