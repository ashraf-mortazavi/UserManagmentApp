using System.ComponentModel;
using ManageUsers.Domain;
using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.DTOs
{
    public class UpdateUserRequest
    {
        [Description("نام")]
        public string FirstName { get; set; } = null!;
        [Description("نام خانوادگی")]
        public string LastName { get; set; } = null!;
        [Description("شماره همراه")]
        public string PhoneNumber { get; set; } = null!;
        [Description("کد ملی")]
        public string NationalCode { get; set; } = null!;
        [Description("ایمیل")]
        public string? Email { get; set; }
        [Description("کد پستی")]
        public string? PostalCode { get; set; }
        [Description("کد پرسنلی")]
        public string? PersonalCode { get; set; }
        [Description("فعال/غیرفعال")]
        public bool Enabled { get; set; }
        [Description("تایید شده توسط ادمین/رد شده توسط ادمین")]
        public bool IsApprovedByAdmin { get; set; }
        [Description("سطح دسترسی")]
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
        [Description("منطقه")]
        public int? AreaId { get; set; }
        [Description("ناحیه")]
        public int? ZoneId { get; set; }
        [Description("ستاد")]
        public string? SetadName { get; set; }
        [Description("شناسه نقش های کاربر")]
        public string RoleId { get; set; }
        [Description("تاریخ تولد")]
        public DateTime? BirthDate { get; set; }
    }

    public sealed class UpdateUserResponse : BaseResponse
    {
        public string Id { get; set; } = string.Empty;
    }
}
