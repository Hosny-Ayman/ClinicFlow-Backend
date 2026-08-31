using AutoMapper;
using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Helper;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Appointments.DTOs;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Application.Features.Appointments.DTOs.Responses;
using ClinicFlow.Application.Features.ClinicWorkingHours;
using ClinicFlow.Application.Features.DoctorSchedules;
using ClinicFlow.Application.Features.DoctorVacations;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.Enums;
using ClinicFlow.Domain.Interfaces;
using System.Numerics;

namespace ClinicFlow.Application.Features.Appointments
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IDoctorVacationRepository _doctorVacationRepository;
        private readonly IClinicWorkingHoursService _clinicWorkingHoursService;
        private readonly IDoctorVacationService _doctorVacationService;
        private readonly IDoctorScheduleService _doctorScheduleService;
        private readonly IClinicWorkingHourRepository _clinicWorkingHourRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentQueryService _appointmentQueryService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService, IPatientRepository patientRepository, IDoctorRepository doctorRepository,
            IClinicRepository clinicRepository, IDoctorScheduleRepository doctorScheduleRepository,
            IDoctorVacationRepository doctorVacationRepository, IClinicWorkingHoursService clinicWorkingHoursService,
            IDoctorScheduleService doctorScheduleService , IDoctorVacationService doctorVacationService
            , IClinicWorkingHourRepository clinicWorkingHourRepository, IInvoiceRepository invoiceRepository,
            IPaymentRepository PaymentRepository, IAppointmentQueryService appointmentQueryService)
        {
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _clinicRepository = clinicRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorVacationRepository = doctorVacationRepository;
            _clinicWorkingHoursService = clinicWorkingHoursService;
            _doctorVacationService = doctorVacationService;
            _doctorScheduleService = doctorScheduleService;
            _clinicWorkingHourRepository = clinicWorkingHourRepository;
            _invoiceRepository = invoiceRepository;
            _paymentRepository = PaymentRepository;
            _appointmentQueryService = appointmentQueryService;

        }

       
        public async Task<OperationResult<int>> AddAppointmentAsync(CreateAndEditAppointmentDto request)
        {

            var validationResult = await ValidateAppointmentBookingAsync<int>(request,_currentUserService.ClinicId!.Value);

            if (!validationResult.IsSuccess)
            {
                return validationResult; 
            }

            var appointment = _mapper.Map<Appointment>(request);

            appointment.ClinicId = _currentUserService.ClinicId!.Value;

            await _appointmentRepository.AddAppointmentAsync(appointment);

            if(request.Status == AppointmentStatusEnum.CheckedIn)
            {
                var result = await CreateInvoiceAndPayment<int>(request.DoctorId, appointment);

                if(!result.IsSuccess)
                {
                    return result;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            
            return OperationResult<int>.Success(appointment.Id);

        }

        private async Task<OperationResult<T>> CreateInvoiceAndPayment<T>(int DoctorId, Appointment appointment)
        {

            var doctor = await _doctorRepository.GetDoctorByIdAsync(DoctorId, _currentUserService.ClinicId!.Value);

            if (doctor == null)
            {
                return OperationResult<T>.NotFound(GeneralErrors.NotFound("الطبيب غير موجود في هذة العيادة"));
            }

            var invoice = new Invoice
            {
                Appointment = appointment,
                SubTotal = doctor.ConsultationFee,
                DiscountAmount = 0,
                TotalAmount = doctor.ConsultationFee,
                Status = PaymentStatusEnum.Paid,
                IssuedAt = DateTime.UtcNow
            };

            var payment = new Payment
            {
                Invoice = invoice,
                Amount = doctor.ConsultationFee,
                Status = PaymentStatusEnum.Paid,
                PaidAt = DateTime.UtcNow,
                PaymentMethodId = (int)PaymentMethodEnum.Cash
            };

            await _invoiceRepository.AddInvoiceAsync(invoice);
            await _paymentRepository.AddPaymentAsync(payment);

            return OperationResult<T>.Success(default!);
        }

        private async Task<OperationResult<T>> ValidateAppointmentBookingAsync<T>(CreateAndEditAppointmentDto request, int clinicId)
        {

            if(! await _doctorRepository.IsDoctorBelongToClinic(request.DoctorId, clinicId))
            {
                return OperationResult<T>.NotFound(GeneralErrors.NotFound("الدكتور غير موجود في هذي العيادة"));
            }

            if (!await _patientRepository.IsPatientInClinicAsync(request.PatientId, clinicId))
            {
                return OperationResult<T>.NotFound(GeneralErrors.NotFound("المريض غير مسجل في العيادة"));
            }

            if (!await _clinicWorkingHoursService.IsTheClinicOpenAtThisAppointmentInsideProject(new Bookappointment
            {
                Day = request.AppointmentDate.DayOfWeek,
                StartTime = request.StartTime
            }))
            {
                return OperationResult<T>.BadRequest(GeneralErrors.BadRequest("العيادة مغلقة في هذا الوقت"));
            }

            if (await _doctorVacationService.HasDoctorVacationOnDate(request.AppointmentDate, request.DoctorId))
            {
                return OperationResult<T>.BadRequest(GeneralErrors.BadRequest("الطبيب في إجازة في هذا الوقت"));
            }

            if (!await _doctorScheduleService.IsDoctorScheduleAvailableAsync(request.AppointmentDate.DayOfWeek, request.DoctorId, request.StartTime))
            {
                return OperationResult<T>.BadRequest(GeneralErrors.BadRequest("الطبيب غير متاح في هذا الوقت"));
            }

            if (await _appointmentRepository.IsAppointmentReservedAsync(request.AppointmentDate, request.StartTime, clinicId, request.DoctorId))
            {
                return OperationResult<T>.Conflict(GeneralErrors.Conflict("الطبيب لديه موعد آخر في هذا الوقت"));
            }

            if(DateTime.Now > request.AppointmentDate.ToDateTime(request.StartTime))
            {
                return OperationResult<T>.BadRequest(GeneralErrors.BadRequest("الوقت المحدد للموعد قد مضى، يرجى اختيار وقت آخر"));
            }

            return OperationResult<T>.Success(default!);
        }

        public async Task<OperationResult<List<SlotDto>>> GetDoctorAvailableSlotsByDateAsync(DoctorAvailableSlotsDtoRequest request)
        {
            var clinicId = _currentUserService.ClinicId!.Value;

            if (!await _doctorRepository.IsDoctorBelongToClinic(request.doctorId, clinicId))
            {
                return OperationResult<List<SlotDto>>.NotFound(GeneralErrors.NotFound("الدكتور غير موجود في هذة العيادة"));
            }

            if (await _doctorVacationService.HasDoctorVacationOnDate(request.appointmentDate, request.doctorId))
            {
                return OperationResult<List<SlotDto>>.BadRequest(GeneralErrors.BadRequest("الطبيب غير متوفر اليوم"));
            }

            var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleAsync(request.appointmentDate.DayOfWeek, request.doctorId, clinicId);

            if (doctorSchedule == null)
            {
                return OperationResult<List<SlotDto>>.NotFound(GeneralErrors.NotFound("لا يوجد جدول زمني للطبيب في هذا اليوم"));
            }

            var appointments = await _appointmentRepository.GetAllAppointmentsAsync(request.appointmentDate, clinicId, request.doctorId);

            var clinicWorkingHours = await _clinicWorkingHourRepository.GetWorkingHoursAndDaysByDayOfWeekAsync(clinicId, request.appointmentDate.DayOfWeek);

            if (clinicWorkingHours == null)
            {
                return OperationResult<List<SlotDto>>
                    .NotFound(GeneralErrors.NotFound("لا يوجد عمل اليوم للعيادة"));
            }

            var appointmentsDictionary = appointments.Where(x=>x.Status != AppointmentStatusEnum.Cancelled).ToDictionary(x => x.StartTime, x => x);

            List<SlotDto> slots = new();
            var currentTime = doctorSchedule.StartTime!.Value;
            var duration = clinicWorkingHours.AppointmentDurationInMinutes;

            while (currentTime.AddMinutes(duration) <= doctorSchedule.EndTime)
            {
                var endTime = currentTime.AddMinutes(duration);
                bool hasAppointment = appointmentsDictionary.TryGetValue(currentTime, out var appointment);

                slots.Add(new SlotDto
                {
                    AppointmentDate = request.appointmentDate,
                    StartTime = currentTime,
                    EndTime = endTime,
                    Status = GetSlotStatus(request.appointmentDate, currentTime, hasAppointment ? appointment.Status : null)
                });

                currentTime = endTime;
            }

            return OperationResult<List<SlotDto>>.Success(slots);
        }

        private SlotStatus GetSlotStatus(DateOnly date, TimeOnly time, AppointmentStatusEnum? status)
        {
            if (status is AppointmentStatusEnum.Scheduled or AppointmentStatusEnum.CheckedIn or AppointmentStatusEnum.InProgress or AppointmentStatusEnum.Completed)
            {
                return SlotStatus.Booked;
            }

            if (DateTime.Now >= date.ToDateTime(time))
            {
                return SlotStatus.Unavailable;
            }

            return SlotStatus.Available;
        }

        public async Task<OperationResult<PagedResponse<GetAllAppointmentDtoResponse>>> GetAllAppointmentAsync(AppointmentSearchDtoRequest request)
        {
            var respons = await _appointmentQueryService.GetAllAppointmentAsync(request, _currentUserService.ClinicId!.Value);

            return  OperationResult<PagedResponse<GetAllAppointmentDtoResponse>>.Success(respons);
        }

        public async Task<OperationResult<GetAdminDashboardStatisticsDtoResponse>> GetAdminDashboardStatisticsAsync()
        {
            var clinicId = _currentUserService.ClinicId!.Value;
            var today = DateOnly.FromDateTime(DateTime.Now);

            var response = await _appointmentQueryService.GetAdminDashboardStatisticsAsync(clinicId, today);


            return OperationResult<GetAdminDashboardStatisticsDtoResponse>.Success(response);
        }

        public async Task<OperationResult<bool>> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatusEnum status)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId, _currentUserService.ClinicId!.Value,true);

            if(appointment == null)
            {
                return OperationResult<bool>.NotFound(GeneralErrors.NotFound("لايوجد موعد"));
            }

            if (status == AppointmentStatusEnum.CheckedIn)
            {
                var result = await CreateInvoiceAndPayment<bool>(appointment.DoctorId, appointment);

                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            appointment.Status = status;

            await _unitOfWork.SaveChangesAsync();


            return OperationResult<bool>.Success(true);

        }

        public async Task<OperationResult<GetAppointmentDashboardDtoResponse>> GetAppointmentDashboardAsync(DateOnly date)
        {
            var respons = await _appointmentQueryService.GetAppointmentDashboardAsync(date, _currentUserService.ClinicId!.Value);

            return OperationResult<GetAppointmentDashboardDtoResponse>.Success(respons);
        }

        public async Task<OperationResult<GetAppointmentDashboardDtoResponse>> GetDoctorAppointmentDashboardAsync(int doctorId, DateOnly date)
        {
            if (date < DateOnly.FromDateTime(DateTime.Now))
            {
                return OperationResult<GetAppointmentDashboardDtoResponse>.BadRequest(
                    GeneralErrors.BadRequest("لا يمكن عرض لوحة المواعيد لتاريخ سابق لليوم")
                );
            }

            var respons = await _appointmentQueryService.GetDoctorAppointmentDashboardAsync(doctorId, date, _currentUserService.ClinicId!.Value);

            return OperationResult<GetAppointmentDashboardDtoResponse>.Success(respons);
        }


    }
}
