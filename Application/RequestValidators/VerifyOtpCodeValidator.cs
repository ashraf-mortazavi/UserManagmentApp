using FluentValidation;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators;

public class VerifyOtpCodeValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpCodeValidator()
    {
        RuleFor(o => o.PhoneNumber)
         .SetValidator(validator: new PhoneNumberValidator<VerifyOtpRequest>())
         .WithMessage(nameof(VerifyOtpRequest.PhoneNumber).GetInvalidValueErrorMessage(typeof(VerifyOtpRequest)));

        RuleFor(x => x.OtpCode)
        .NotEmpty()
        .Length(5)
        .WithMessage("OTP Code must be 6 digits")
        .Matches(@"^\d{5}$")
        .WithMessage("OTP Code must be digits fromat.");
    }
}
