using Microsoft.Extensions.Options;
using STech.Data.SettingModels;
using System.Net.Mail;
using System.Net;

namespace STech.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSenderSettings _emailSettings;

        public EmailService(IOptions<EmailSenderSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message, byte[]? fileBytes, string? fileName)
        {
            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                if (fileBytes != null && !string.IsNullOrEmpty(fileName))
                {
                    var stream = new MemoryStream(fileBytes);
                    var attachment = new Attachment(stream, fileName);
                    mailMessage.Attachments.Add(attachment);
                }

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
