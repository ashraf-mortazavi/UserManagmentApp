using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.DTOs
{
    public class UpdateUserProfileRequest
    {
        [Description("ایمیل")]
        public string? Email { get; set; }

        [Description("شماره همراه")]
        public string? PhoneNumber { get; set; }

        [Description("عکس پروفایل")]
        public IFormFile? Avatar { get; set; }
    }

    public sealed class UpdateUserProfileResponse : BaseResponse
    {
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
