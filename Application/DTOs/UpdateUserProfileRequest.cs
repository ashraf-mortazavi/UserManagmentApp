using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.DTOs
{
    public class UpdateUserProfileRequest
    {

        [Description("شماره همراه")]
        public string? PhoneNumber { get; set; }

        [Description("تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        [Description("عکس پروفایل")]
        public IFormFile? Avatar { get; set; }
    }

    public sealed class UpdateUserProfileResponse : BaseResponse
    {
        public string? Id { get; set; }
    }
}
