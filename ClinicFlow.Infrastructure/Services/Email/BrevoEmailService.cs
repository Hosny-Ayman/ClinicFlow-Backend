using ClinicFlow.Application.Common.DTOs;
using ClinicFlow.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ClinicFlow.Infrastructure.Services.Email
{
    public class BrevoEmailService : IEmailService
    {

        private readonly EmailSettings _settings;

        public BrevoEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task SendAsync(EmailMessage message)
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.FromName,
                    _settings.FromEmail));

            email.To.Add(
                MailboxAddress.Parse(message.To));

            email.Subject = message.Subject;

            email.Body = new TextPart("plain")
            {
                Text = message.Body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync( _settings.SmtpServer, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
