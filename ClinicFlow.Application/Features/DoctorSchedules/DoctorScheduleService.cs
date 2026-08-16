using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.DoctorSchedules.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;

namespace ClinicFlow.Application.Features.DoctorSchedules
{
    public class DoctorScheduleService
    {
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorScheduleService(IDoctorScheduleRepository doctorScheduleRepository, IUnitOfWork unitOfWork
            , IMapper mapper, ICurrentUserService currentUserService, IDoctorRepository doctorRepository)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _doctorRepository = doctorRepository;
        }


        public async Task AddDoctorSchedulesInsideProjectAsync(Doctor doctor)
        {

            var doctorSchedules = new List<DoctorSchedule>
            {
                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Sunday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = true
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = true
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Tuesday,
                        StartTime = null,
                        EndTime = null,
                        IsAvailable = false
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Wednesday,
                        StartTime = new TimeOnly(12, 0),
                        EndTime = new TimeOnly(20, 0),
                        IsAvailable = true
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Thursday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsAvailable = true
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Friday,
                        StartTime = null,
                        EndTime = null,
                        IsAvailable = false
                    },

                    new DoctorSchedule
                    {
                        Doctor = doctor,
                        DayOfWeek = DayOfWeek.Saturday,
                        StartTime = new TimeOnly(10, 0),
                        EndTime = new TimeOnly(14, 0),
                        IsAvailable = true
                    }
            };

            await _doctorScheduleRepository.AddDoctorSchedulesAsync(doctorSchedules);


        }

        public async Task<OperationResult<List<UpdateAndGetDoctorScheduleDtoRequest>>> GetAllDoctorSchedulesAsync(int userId)
        {

           int? doctotrId = await _doctorRepository.GetDoctorIdByUserId(userId, _currentUserService.ClinicId!.Value);

            if(doctotrId == null)
            {
                return OperationResult<List<UpdateAndGetDoctorScheduleDtoRequest>>.NotFound(GeneralErrors.NotFound("Doctor not found for the given user."));
            }

            var doctorSchedules = await _doctorScheduleRepository.GetAllDoctorSchedulesAsync(doctotrId.Value, _currentUserService.ClinicId!.Value);

            var respons = _mapper.Map<List<UpdateAndGetDoctorScheduleDtoRequest>>(doctorSchedules);

            return OperationResult<List<UpdateAndGetDoctorScheduleDtoRequest>>.Success(respons);
        }

        public async Task<OperationResult<bool>> UpdateSchedulesInsideProjectAsync(List<UpdateAndGetDoctorScheduleDtoRequest> request , int userId)
        {

            int? doctotrId = await _doctorRepository.GetDoctorIdByUserId(userId, _currentUserService.ClinicId!.Value);

            if (doctotrId == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("Doctor not found for the given user."));
            }


            var doctorSchedules = await _doctorScheduleRepository.GetAllDoctorSchedulesAsync(doctotrId.Value, _currentUserService.ClinicId!.Value,true);

            var doctorSchedulesDictionary = request.ToDictionary(x => x.DayOfWeek);

            foreach (var schedule in doctorSchedules)
            {

                if (doctorSchedulesDictionary.TryGetValue(schedule.DayOfWeek, out var requestSchedule))
                {
                    _mapper.Map(requestSchedule, schedule);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);

        }

    }
}
