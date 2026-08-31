using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Requests;
using ClinicFlow.Application.Features.MedicalRecords.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.Application.Features.MedicalRecords
{
    public class MedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<MedicalRecordService> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<OperationResult<int>> CreateMedicalRecordAsync(CreateMedicalRecordDtoRequest request)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId, clinicId);

            if (appointment == null)
            {
                return OperationResult<int>.NotFound(GeneralErrors.NotFound("الموعد غير موجود"));
            }

            if (appointment.Status != AppointmentStatusEnum.InProgress)
            {
                return OperationResult<int>.BadRequest(GeneralErrors.BadRequest("لا يمكن إنشاء سجل طبي إلا عندما يكون الموعد في حالة كشف"));
            }

            if (await _medicalRecordRepository.HasMedicalRecordForAppointmentAsync(request.AppointmentId))
            {
                return OperationResult<int>.Conflict(GeneralErrors.Conflict("يوجد سجل طبي مسجل لهذا الموعد بالفعل"));
            }

            var medicalRecord = _mapper.Map<MedicalRecord>(request);
            medicalRecord.PatientId = appointment.PatientId;
            medicalRecord.DoctorId = appointment.DoctorId;
            medicalRecord.CreatedAt = DateTime.UtcNow;

            await _medicalRecordRepository.AddMedicalRecordAsync(medicalRecord);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(medicalRecord.Id);
        }

        public async Task<OperationResult<GetMedicalRecordDtoResponse>> GetMedicalRecordByIdAsync(int id)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var record = await _medicalRecordRepository.GetMedicalRecordByIdAsync(id, clinicId);

            if (record == null)
            {
                return OperationResult<GetMedicalRecordDtoResponse>.NotFound(GeneralErrors.NotFound("السجل الطبي غير موجود"));
            }

            var response = _mapper.Map<GetMedicalRecordDtoResponse>(record);

            return OperationResult<GetMedicalRecordDtoResponse>.Success(response);
        }

        public async Task<OperationResult<GetMedicalRecordDtoResponse>> GetMedicalRecordByAppointmentIdAsync(int appointmentId)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var record = await _medicalRecordRepository.GetMedicalRecordByAppointmentIdAsync(appointmentId, clinicId);

            if (record == null)
            {
                return OperationResult<GetMedicalRecordDtoResponse>.NotFound(GeneralErrors.NotFound("لا يوجد سجل طبي لهذا الموعد"));
            }

            var response = _mapper.Map<GetMedicalRecordDtoResponse>(record);

            return OperationResult<GetMedicalRecordDtoResponse>.Success(response);
        }

        public async Task<OperationResult<bool>> UpdateMedicalRecordAsync(UpdateMedicalRecordDtoRequest request)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var record = await _medicalRecordRepository.GetMedicalRecordByIdAsync(request.Id, clinicId, tracking: true);

            if (record == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("السجل الطبي غير موجود"));
            }

            //bool isClinicOwner = _currentUserService.Roles.Contains(nameof(RoleEnum.ClinicOwner));
            //bool isSuperAdmin = _currentUserService.Roles.Contains(nameof(RoleEnum.SuperAdmin));

            //if (!isClinicOwner && !isSuperAdmin)
            //{
            //    int? currentDoctorId = await _doctorRepository.GetDoctorIdByUserId(_currentUserService.UserId!.Value, clinicId);

            //    if (currentDoctorId == null || currentDoctorId.Value != record.DoctorId)
            //    {
            //        return OperationResult<bool>.Forbidden();
            //    }
            //}

            _mapper.Map(request, record);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }
    }
}
