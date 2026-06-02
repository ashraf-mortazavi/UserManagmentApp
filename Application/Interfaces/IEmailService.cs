namespace ManageUsers.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string userName, string resetLink);
        Task<bool> SendEmailAsync(string email, string subject, string htmlContent);
    }
}
