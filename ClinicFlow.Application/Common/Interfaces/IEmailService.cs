using ClinicFlow.Application.Common.DTOs;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IEmailService
    {

        Task SendAsync(EmailMessage message);

    }
}
