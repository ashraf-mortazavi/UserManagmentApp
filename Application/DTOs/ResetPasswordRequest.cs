using System.ComponentModel.DataAnnotations;

namespace ManageUsers.Application.DTOs;

public record ResetPasswordRequest(string Email,string NewPassword,string ConfirmPassword);


public class ResetPasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string FailedResult { get; set; } = string.Empty;
}
