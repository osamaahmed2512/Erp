using Application.Interfaces.ExternalServices;
using Infrastructure.Setting;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Email));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_settings.Email, _settings.Password);
                await smtp.SendAsync(email);
            }catch(Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            await smtp.DisconnectAsync(true);
        }
    }
}
