using ManageUsers.Application.DTOs;
using MediatR;
using System.ComponentModel;

namespace ManageUsers.Application.Commands;

public class RequestOtpCodeCommand(string phoneNumber) : IRequest<RequestOTPCodeResponse>
{
    [Description("تلفن")]
    public string PhoneNumber { get; set; } = phoneNumber;

}
