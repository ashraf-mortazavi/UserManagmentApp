namespace ManageUsers.Application.DTOs;

public class AdminResetPasswordResponse
{
    public string GeneratedPassword { get; set; } = string.Empty;
    public string FailedResult { get; set; } = string.Empty;
}
