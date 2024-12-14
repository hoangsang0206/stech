namespace STech.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message, byte[]? fileBytes, string? fileName);
    }
}
