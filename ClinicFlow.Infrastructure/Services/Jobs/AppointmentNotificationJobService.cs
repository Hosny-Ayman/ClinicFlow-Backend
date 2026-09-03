using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Interfaces.Jobs;
using ClinicFlow.Domain.Interfaces;

namespace ClinicFlow.Infrastructure.Services.Jobs
{
    public class AppointmentNotificationJobService : IAppointmentNotificationJobService
    {

        private readonly IEmailService _emailService;
        private readonly IAppointmentQueryService _appointmentQueryService;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentNotificationJobService(IEmailService emailService,
            IAppointmentQueryService appointmentQueryService, ICurrentUserService currentUserService,
            IAppointmentRepository appointmentRepository, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _appointmentQueryService = appointmentQueryService;
            _appointmentRepository = appointmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task SendAppointmentReminderAsync()
        {
            var appointments = await _appointmentQueryService.GetAllCloseToStartAppointments();

            foreach (var appointment in appointments)
            {
                var emailMessage = new EmailMessage
                {
                    To = appointment.PatientEmail,
                    Subject = "تذكير بالموعد",
                    Body = $"عزيزي/عزيزتي {appointment.PatientName}،\n\nهذا تذكير بموعدك المحدد يوم {appointment.AppointmentDate} في تمام الساعة {appointment.AppointmentTime}.\n\nشكرًا لك."
                };

                if(!string.IsNullOrWhiteSpace(appointment.PatientEmail))
                {
                    await _emailService.SendAsync(emailMessage);
                }

              

                var appointmentDetails = await _appointmentRepository.GetAppointmentByIdAsync(appointment.Id, true);

                if(appointmentDetails!=null)
                {
                    appointmentDetails.ReminderSent = true;

                }
               
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
