using System.ComponentModel;

namespace ManageUsers.Application.DTOs
{
    public class VerifyOtpRequest
    {
        [Description("تلفن")]
        public string PhoneNumber { get; set; }

        [Description("کد")]
        public string OtpCode { get; set; }
    }
}
