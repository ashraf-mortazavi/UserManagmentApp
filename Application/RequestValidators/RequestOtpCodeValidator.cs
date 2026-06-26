using FluentValidation;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators;

public class RequestOtpCodeValidator : AbstractValidator<RequestOTPCode>
{
    public RequestOtpCodeValidator()
    {
       RuleFor(o => o.PhoneNumber)
        .SetValidator(validator: new PhoneNumberValidator<RequestOTPCode>())
        .WithMessage(nameof(RequestOTPCode.PhoneNumber).GetInvalidValueErrorMessage(typeof(RequestOTPCode)));
    }
}
