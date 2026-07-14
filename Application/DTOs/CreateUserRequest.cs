
using System.ComponentModel;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.DTOs
{
    public class CreateUserRequest
    {
        [Description("نام")]
        public string FirstName { get; set; }
        [Description("نام خانوادگی")]
        public string LastName { get; set; }
        [Description("شماره همراه")]
        public string PhoneNumber { get; set; }
        [Description("کد ملی")]
        public string NationalCode  { get; set; }
        [Description("ایمیل")]
        public string? Email {  get; set; }
        [Description("کد پستی")]
        public string? PostalCode { get; set; }
        [Description("نام کاربری")]
        public string UserName { get; set; }
        [Description("رمز عبور")]
        public string Password { get; set; }
        [Description("کد پرسنلی")]
        public string? PersonalCode { get; set; }
        [Description("سطح دسترسی")]
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
        [Description("منطقه")]
        public int? ZoneId {get; set; }
        [Description("ناحیه")]
        public int? AreaId { get; set; }
        [Description("شناسه نقش های کاربر")]
        public List<string> UserRoleIds { get; set; }
        [Description("عکس پروفایل")]
        public IFormFile? Avatar { get; set; }

    }

    public sealed class CreateUserResponse : BaseResponse
    {
        public string Id { get; set; }
    }

}