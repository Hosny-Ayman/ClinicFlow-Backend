using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Clinics.DTOs.Requests;
using ClinicFlow.Application.Features.Clinics.DTOs.Responses;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace ClinicFlow.Application.Features.Clinics
{
    public class ClinicService
    {

        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IOwnershipService _ownershipService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClinicQueryService _queryService;
        private readonly IUserRoleRepository _roleRepository;
        private readonly UserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IClinicSetupRepository _clinicSetupRepository;


        public ClinicService(IClinicRepository clinicRepository,IUserRepository userRepository,IMapper mapper, IOwnershipService ownershipService
            , IUnitOfWork unitOfWork, IClinicQueryService QueryService, IUserRoleRepository RoleRepository,UserService userService
            , ICurrentUserService currentUserService, IFileStorageService fileStorageService, IClinicSetupRepository clinicSetupRepository)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _ownershipService = ownershipService;
            _unitOfWork = unitOfWork;
            _queryService = QueryService;
            _roleRepository = RoleRepository;
            _userService = userService;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
            _clinicSetupRepository = clinicSetupRepository;
        }

      

        public async Task<OperationResult<CreateClinicResponse>> CreateClinicAsync(CreateAndEditClinicDtoRequest clinicDto, CreateAndEditUserDtoRequest userDto)
        {
            string? imageId = null;
            var user = await _userService.AddUserInsideProjectOnlyAsync(userDto);

            if(user == null)
            {
                return OperationResult<CreateClinicResponse>.BadRequest();
            }

            var clinic = _mapper.Map<Clinic>(clinicDto);

            if (clinicDto.LogoUrl != null)
            {
                imageId = await _fileStorageService.UploadImageAsync(clinicDto.LogoUrl);
            }

            clinic.LogoUrl = imageId;
            user.Clinic = clinic;

            var clinicSetup = new ClinicSetup
            {
                HasSkippedSetup = false,
                clinic = clinic
            };

            await _roleRepository.AssignRoleAsync(user, RoleEnum.ClinicOwner);

            await _clinicRepository.AddAsync(clinic);

            await _userRepository.AddAsync(user);

            await _clinicSetupRepository.AddClinicSetupStatusAsync(clinicSetup);

            await _unitOfWork.SaveChangesAsync();

            var requst = await _queryService.GetClinicInfoWithOwnerFullnameAsync(clinic.Id);

            if (requst == null)
            {
                return OperationResult<CreateClinicResponse>.Failure(GeneralErrors.Failure("Create Clinic Failed Try Later"));
            }

            return OperationResult<CreateClinicResponse>.Success(requst);

        }

        public async Task<OperationResult<bool>> UpdateClinicAsync(CreateAndEditClinicDtoRequest clinicDto)
        {

            string? oldImage = null;
            string? newImage = null;

            try
            {
                var clinic = await _clinicRepository.GetClinicByIdAsync(_currentUserService.ClinicId!.Value, true);

                if (clinic == null)
                {
                    return OperationResult<bool>.NotFound();
                }

                oldImage = clinic.LogoUrl;

                _mapper.Map(clinicDto, clinic);

                if(clinicDto.LogoUrl!=null)
                {
                    newImage = await _fileStorageService.UploadImageAsync(clinicDto.LogoUrl);
                    clinic.LogoUrl = newImage;
                }
                else
                {
                    if (clinicDto.IsImageDelted)
                    {
                        clinic.LogoUrl = null;
                    }

                }

                await _unitOfWork.SaveChangesAsync();

                if(clinicDto.IsImageDelted && !string.IsNullOrEmpty(oldImage))
                {
                    await _fileStorageService.DeleteImageAsync(oldImage);
                }

                return OperationResult<bool>.Success(true);
            }
            catch
            {

                if(clinicDto.LogoUrl!=null)
                {
                    await _fileStorageService.DeleteImageAsync(newImage!);
                }
               
                throw;
            }
           
        }

        public async Task<OperationResult<GetClinicDtoResponse>> GetClinicAsync()
        {

            var clinic = await _clinicRepository.GetClinicByIdAsync(_currentUserService.ClinicId!.Value);

            if (clinic == null)
            {
                return OperationResult<GetClinicDtoResponse>.NotFound();
            }

            var response =  _mapper.Map<GetClinicDtoResponse>(clinic);

            if(!string.IsNullOrEmpty(clinic.LogoUrl))
            {
                response.LogoUrl = _fileStorageService.GetFileUrl(clinic.LogoUrl!);
            }


            return OperationResult<GetClinicDtoResponse>.Success(response);
        }


    }
}
