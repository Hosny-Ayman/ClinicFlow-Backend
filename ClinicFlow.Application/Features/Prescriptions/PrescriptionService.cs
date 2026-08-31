using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Common.Security;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Requests;
using ClinicFlow.Application.Features.Prescriptions.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.Application.Features.Prescriptions
{
    public class PrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICheckService _checkService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            ICheckService checkService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _checkService = checkService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<OperationResult<int>> CreatePrescriptionAsync(CreatePrescriptionDtoRequest request)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var medicalRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(request.MedicalRecordId, clinicId);

            if (medicalRecord == null)
            {
                return OperationResult<int>.NotFound(GeneralErrors.NotFound("السجل الطبي غير موجود"));
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(medicalRecord.AppointmentId, clinicId);

            if (appointment == null)
            {
                return OperationResult<int>.NotFound(GeneralErrors.NotFound("الموعد غير موجود"));
            }

            if (appointment.Status != AppointmentStatusEnum.InProgress)
            {
                return OperationResult<int>.BadRequest(GeneralErrors.BadRequest("لا يمكن إنشاء روشتة إلا عندما يكون الموعد في حالة كشف"));
            }

            if (await _prescriptionRepository.HasPrescriptionForMedicalRecordAsync(request.MedicalRecordId))
            {
                return OperationResult<int>.Conflict(GeneralErrors.Conflict("يوجد روشتة مسجلة لهذا السجل الطبي بالفعل"));
            }

            if (request.PrescriptionItems == null || request.PrescriptionItems.Count == 0)
            {
                return OperationResult<int>.BadRequest(GeneralErrors.BadRequest("يجب إضافة دواء واحد على الأقل للروشتة"));
            }

            var prescription = _mapper.Map<Prescription>(request);
            prescription.MedicalRecordId = medicalRecord.Id;
            prescription.DoctorId = medicalRecord.DoctorId;
            prescription.IssuedAt = DateTime.UtcNow;

            await _prescriptionRepository.AddPrescriptionAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(prescription.Id);
        }

        public async Task<OperationResult<GetPrescriptionDtoResponse>> GetPrescriptionByIdAsync(int id)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(id, clinicId);

            if (prescription == null)
            {
                return OperationResult<GetPrescriptionDtoResponse>.NotFound(GeneralErrors.NotFound("الروشتة غير موجودة"));
            }

            var response = _mapper.Map<GetPrescriptionDtoResponse>(prescription);

            return OperationResult<GetPrescriptionDtoResponse>.Success(response);
        }

        public async Task<OperationResult<GetPrescriptionDtoResponse>> GetPrescriptionByMedicalRecordIdAsync(int medicalRecordId)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var prescription = await _prescriptionRepository.GetPrescriptionByMedicalRecordIdAsync(medicalRecordId, clinicId);

            if (prescription == null)
            {
                return OperationResult<GetPrescriptionDtoResponse>.NotFound(GeneralErrors.NotFound("لا يوجد روشتة لهذا السجل الطبي"));
            }

            var response = _mapper.Map<GetPrescriptionDtoResponse>(prescription);

            return OperationResult<GetPrescriptionDtoResponse>.Success(response);
        }

        public async Task<OperationResult<bool>> UpdatePrescriptionAsync(UpdatePrescriptionDtoRequest request)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(request.Id, clinicId, tracking: true);

            if (prescription == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("الروشتة غير موجودة"));
            }

            var doctorUser = await _userRepository.GetUserByDoctorIdAsync(prescription.DoctorId, clinicId);

            if (doctorUser == null || !_checkService.EnsureCanManageUser(doctorUser.Id))
            {
                return OperationResult<bool>.Forbidden();
            }

            if (request.PrescriptionItems == null || request.PrescriptionItems.Count == 0)
            {
                return OperationResult<bool>.BadRequest(GeneralErrors.BadRequest("يجب إضافة دواء واحد على الأقل للروشتة"));
            }

            prescription.Notes = request.Notes;

            var existingItemsDict = prescription.PrescriptionItems.ToDictionary(x => x.Id);
            var incomingIds = new HashSet<int>();

            foreach (var itemDto in request.PrescriptionItems)
            {
                if (itemDto.Id == 0)
                {
                    var newItem = _mapper.Map<PrescriptionItem>(itemDto);
                    prescription.PrescriptionItems.Add(newItem);
                }
                else
                {
                    if (!existingItemsDict.TryGetValue(itemDto.Id, out var existingItem))
                    {
                        return OperationResult<bool>.BadRequest(GeneralErrors.BadRequest("عنصر الروشتة غير موجود في هذه الروشتة"));
                    }

                    _mapper.Map(itemDto, existingItem);
                    incomingIds.Add(itemDto.Id);
                }
            }

            var itemsToRemove = prescription.PrescriptionItems
                .Where(x => x.Id > 0 && !incomingIds.Contains(x.Id))
                .ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                prescription.PrescriptionItems.Remove(itemToRemove);
            }

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }
    }
}
