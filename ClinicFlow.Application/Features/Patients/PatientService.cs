using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Application.Features.Patients.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.Application.Features.Patients
{
    public class PatientService
    {

        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PatientService> _logger;
        private readonly IPatientQueryService _queryService;

        public PatientService(
            IPatientRepository patientRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<PatientService> logger,
            IPatientQueryService queryService)
        {
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _logger = logger;
            _queryService = queryService;
        }


        public async Task<OperationResult<int>> CreatePatientAsync(CreatePatientDtoRequest dto)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var person = new Person
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var patient = _mapper.Map<Patient>(dto);
            patient.Person = person;
            patient.CreatedAt = DateTime.UtcNow;

            var clinicPatient = new ClinicPatient
            {
                ClinicId = clinicId,
                Patient = patient,
                FirstVisitDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            patient.ClinicPatients.Add(clinicPatient);

            await _patientRepository.AddPatientAsync(patient);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<int>.Success(patient.Id);
        }

        public async Task<OperationResult<GetPatientDtoResponse>> GetPatientByIdAsync(int patientId)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var patient = await _patientRepository.GetPatientByIdAsync(patientId, clinicId);

            if (patient == null)
            {
                return OperationResult<GetPatientDtoResponse>.NotFound(GeneralErrors.NotFound("Patient"));
            }

            var dto = _mapper.Map<GetPatientDtoResponse>(patient);

            dto.Gender = patient.Gender.ToString();
            dto.BloodType = patient.BloodType?.ToString();

            return OperationResult<GetPatientDtoResponse>.Success(dto);
        }

        public async Task<OperationResult<bool>> UpdatePatientAsync(UpdatePatientDtoRequest dto)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            var patient = await _patientRepository.GetPatientByIdAsync(dto.Id, clinicId, tracking: true);

            if (patient == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("Patient Update Failed: Patient"));
            }

            patient.Person.FirstName = dto.FirstName;
            patient.Person.LastName = dto.LastName;
            patient.Person.Email = dto.Email;
            patient.Person.PhoneNumber = dto.PhoneNumber;

            _mapper.Map(dto, patient);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult<bool>.Success(true);
        }

        public async Task<OperationResult<PagedResponse<GetAllPatientsDtoResponse>>> GetAllPatientsAsync(PatientSearchDtoRequest request)
        {
            var response = await _queryService.GetAllPatientsAsync(request, _currentUserService.ClinicId!.Value);

            return OperationResult<PagedResponse<GetAllPatientsDtoResponse>>.Success(response);
        }

    }
}
