using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(nameof(ForgotPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ForgotPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ForgotPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ForgotPasswordRequest)));
            

            RuleFor(o => o.Email)
                .SetValidator(validator: new EmailValidator<ForgotPasswordRequest>()!)
                .When(o => !string.IsNullOrEmpty(o.Email))
                .WithMessage(nameof(ForgotPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ForgotPasswordRequest)));
        }
    }
}