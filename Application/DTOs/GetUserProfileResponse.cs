using ManageUsers.Domain;

namespace ManageUsers.Application.DTOs;

public class GetUserProfileResponse : BaseResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string? RoleName { get; set; }
}
