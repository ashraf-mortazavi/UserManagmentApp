using ManageUsers.Application.DTOs;
using MediatR;
using System.ComponentModel;

namespace ManageUsers.Application.Commands;

public class VerifyOtpCodeCommand(string phoneNumber, string otpCode) : IRequest<VerifyOtpResponse>
{
    public string PhoneNumber { get; set; } = phoneNumber;
    public string OTPCode { get; set; } = otpCode;
}
