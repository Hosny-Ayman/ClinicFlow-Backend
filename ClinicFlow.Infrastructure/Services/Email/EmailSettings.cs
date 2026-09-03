namespace ClinicFlow.Infrastructure.Services.Email
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = default!;

    }
}
