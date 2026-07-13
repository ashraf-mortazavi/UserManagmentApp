using System.ComponentModel;
using ManageUsers.Domain;

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
        [Description("موقعیت")]
        public string? Position { get; set; }
        [Description("توضیحات")]
        public string? Description { get; set; }
        [Description("فعال/غیرفعال")]
        public bool Enabled { get; set; }
        [Description("سطح دسترسی")]
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
        [Description("شناسه سازمان")]
        public int? OrganizationId { get; set; }
        [Description("منطقه")]
        public int? AreaId { get; set; }
        [Description("ناحیه")]
        public int? RegionId { get; set; }
        [Description("شناسه نقش های کاربر")]
        public List<string> UserRoleIds { get; set; } = new();
    }

    public sealed class UpdateUserResponse : BaseResponse
    {
        public string Id { get; set; } = string.Empty;
    }
}
