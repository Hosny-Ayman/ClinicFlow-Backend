using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.InterFaces;

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

        public DoctorService(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork, IMapper mapper,
            IFileStorageService FileStorageService, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = FileStorageService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }


        public async Task<OperationResult<int>> CreateDoctorAsync(CreateDoctorDtoRequest doctorDto)
        {
            string? imageId = null;

            try
            {
                var user = await _userRepository.GetUserByIdAsync(doctorDto.UserId, true);

                if (user == null)
                {
                    return OperationResult<int>
                        .NotFound(GeneralErrors.NotFound("User Not Found"));
                }


                await _userRoleRepository.AssignRoleAsync(doctorDto.UserId, RoleEnum.Doctor);


                if (doctorDto.ProfileImage != null)
                {
                    imageId = await _fileStorageService.UploadImageAsync(
                        doctorDto.ProfileImage);
                }


                var doctor = _mapper.Map<Doctor>(doctorDto);

                doctor.ProFileImageid = imageId;

                
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

    }
}
