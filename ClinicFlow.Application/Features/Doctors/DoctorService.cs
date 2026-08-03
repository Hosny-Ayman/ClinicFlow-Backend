using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Application.Features.Doctors.DTOs.Responses;
using ClinicFlow.Application.Features.Users;
using ClinicFlow.Application.Features.Users.DTOs.Requests;
using ClinicFlow.Application.Features.Users.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.Application.Features.Doctors
{
    public class DoctorService
    {

        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserService _userService;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork, IMapper mapper,
            IFileStorageService FileStorageService, IUserRepository userRepository,
            IUserRoleRepository userRoleRepository, ICurrentUserService currentUserService,
            UserService userService, ISpecialtyRepository specialtyRepository, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = FileStorageService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _currentUserService = currentUserService;
            _userService = userService;
            _logger = logger;

        }


        public async Task<OperationResult<int>> CreateDoctorStepsAsync(CreateAndEditDoctorDtoRequest doctorDto)
        {
            string? imageId = null;

            try
            {
                var user = await _userRepository.GetUserByIdAsync(_currentUserService.UserId!.Value, _currentUserService.ClinicId!.Value, true);

                if (user == null)
                {
                    return OperationResult<int>.NotFound(GeneralErrors.NotFound("User Not Found"));
                }

                if(await _userRoleRepository.HasRoleAsync(_currentUserService.UserId!.Value,RoleEnum.Doctor))
                {
                    return OperationResult<int>.Conflict(GeneralErrors.Conflict("User Already Has This Roles"));
                }
                await _userRoleRepository.AssignRoleAsync(_currentUserService.UserId!.Value, RoleEnum.Doctor);


                if (doctorDto.ProfileImage != null)
                {
                    imageId = await _fileStorageService.UploadImageAsync(
                        doctorDto.ProfileImage);
                }


                var doctor = _mapper.Map<Doctor>(doctorDto);

                doctor.UserId = _currentUserService.UserId!.Value;

                doctor.ClinicId = _currentUserService.ClinicId!.Value;

                doctor.ProfileImageUrl = imageId;

                
                await _doctorRepository.AddDoctorAsync(doctor);

                await _unitOfWork.SaveChangesAsync();


                return OperationResult<int>.Success(doctor.Id);
            }
            catch
            {
                if (imageId != null)
                {
                    await _fileStorageService.DeleteImageAsync(imageId);
                }

                throw;
            }
        }

        public async Task<OperationResult<int>> CreateDoctorAsync(CreateAndEditDoctorDtoRequest doctorDto,CreateAndEditUserDtoRequest userDto)
        {
            string? imageId = null;

            try
            {
                var user = await _userService.AddUserAsync(userDto);

                if (user == null)
                {
                    return OperationResult<int>.Conflict(GeneralErrors.Conflict("User Is Already Exists"));
                }

                if (await _userRoleRepository.HasRoleAsync(_currentUserService.UserId!.Value, RoleEnum.Doctor))
                {
                    return OperationResult<int>.Conflict(GeneralErrors.Conflict("User Already Has This Role"));
                }
                await _userRoleRepository.AssignRoleAsync(_currentUserService.UserId!.Value, RoleEnum.Doctor);


                if (doctorDto.ProfileImage != null)
                {
                    imageId = await _fileStorageService.UploadImageAsync(
                        doctorDto.ProfileImage);
                }


                var doctor = _mapper.Map<Doctor>(doctorDto);

                doctor.UserId = _currentUserService.UserId!.Value;

                doctor.ClinicId = _currentUserService.ClinicId!.Value;

                doctor.ProfileImageUrl = imageId;


                await _doctorRepository.AddDoctorAsync(doctor);

                await _unitOfWork.SaveChangesAsync();


                return OperationResult<int>.Success(doctor.Id);
            }
            catch
            {
                if (imageId != null)
                {
                    await _fileStorageService.DeleteImageAsync(imageId);
                }

                throw;
            }
        }

        public async Task<OperationResult<GetDoctorFullInforamtionDtoResponse>> GetDoctorFullInforamtionByIdAsync(int DoctorId)
        {

            var user = await _userRepository.GetUserByDoctorIdAsync(DoctorId, _currentUserService.ClinicId!.Value);

            if (user == null )
            {
                OperationResult<GetDoctorFullInforamtionDtoResponse>.NotFound(GeneralErrors.NotFound("Doctor Not Found"));
            }

            var doctor = await _doctorRepository.GetDoctorByIdAsync(DoctorId, _currentUserService.ClinicId!.Value);

            if(doctor == null)
            {
                OperationResult<GetDoctorFullInforamtionDtoResponse>.NotFound(GeneralErrors.NotFound("Doctor Not Found"));
            }


            var userDto = _mapper.Map<GetUserInformationDtoResponse>(user);

            var doctorDto = _mapper.Map<GetDoctorInforamtionDtoResponse>(doctor);

            doctorDto.Gender = doctor!.Gender.ToString();
            doctorDto.SpecialtieName = doctor.Specialty.Name;
            doctorDto.ProfileImageUrl = doctor.ProfileImageUrl is null ? null:_fileStorageService.GetFileUrl(doctorDto.ProfileImageUrl!);

            var respons = new GetDoctorFullInforamtionDtoResponse
            {
                User = userDto,
                Doctor = doctorDto
            };

            return OperationResult<GetDoctorFullInforamtionDtoResponse>.Success(respons);


        }

        public async Task<OperationResult<bool>> UpdateDoctorAsync(UpdateUserInformationDtoRequest userDto, UpdateDoctorInforamtionDtoRequest doctorDto)
        {
            string? oldImage = null;
            string? newImage = null;
            try
            {

                var user = await _userRepository.GetUserByIdAsync(userDto.Id, _currentUserService.ClinicId!.Value, true);

                if(user == null)
                {
                    return OperationResult<bool>.NotFound(GeneralErrors.NotFound("Doctor Update Failed Doctor Not Found"));
                }

                var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorDto.Id, _currentUserService.ClinicId!.Value, true);

                if (doctor == null)
                {
                    return OperationResult<bool>.NotFound(GeneralErrors.NotFound("Doctor Update Failed Doctor Not Found"));
                }

                oldImage = doctor.ProfileImageUrl;

                await _userService.UpdateUserAsync(user, userDto);

                _mapper.Map(doctorDto, doctor);

                doctor.ProfileImageUrl = oldImage;

                if (doctorDto.ProfileImageUrl is not null )
                {
                    newImage = await _fileStorageService.UploadImageAsync(doctorDto.ProfileImageUrl);
                    doctor.ProfileImageUrl = newImage;
                }
                else
                {
                    if (doctorDto.IsImageDeleted)
                    {
                        doctor.ProfileImageUrl = null;
                    }
                   
                }

               
                await _unitOfWork.SaveChangesAsync();

                if (doctorDto.IsImageDeleted)
                {
                    await DeleteOldImageAsync(oldImage);
                }
                    
                
                    

                return OperationResult<bool>.Success(true);
            }
            catch
            {
                await RollbackUploadedImage(newImage);
                throw;
            }


        }


        private async Task DeleteOldImageAsync(string? oldImage)
        {
            if (string.IsNullOrWhiteSpace(oldImage))
                return;

            var deleted = await _fileStorageService.DeleteImageAsync(oldImage);

            if (!deleted)
            {
                _logger.LogWarning( "Failed to delete image {Image}",oldImage);
            }
        }

        private async Task RollbackUploadedImage(string? newImage)
        {
            if (string.IsNullOrWhiteSpace(newImage))
                return;

            await _fileStorageService.DeleteImageAsync(newImage);
        }



    }
}
