using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(nameof(ResetPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ResetPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)));


            RuleFor(o => o.Email)
                .SetValidator(validator: new EmailValidator<ResetPasswordRequest>()!)
                .When(o => !string.IsNullOrEmpty(o.Email))
                .WithMessage(nameof(ResetPasswordRequest.Email).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)));

            RuleFor(o => o.NewPassword)
                .NotEmpty()
                .WithMessage(nameof(ResetPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ResetPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)));



            RuleFor(o => o.NewPassword)
               .SetValidator(validator: new PasswordValidator<ResetPasswordRequest>())
               .WithMessage(nameof(ResetPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(nameof(ResetPasswordRequest.ConfirmPassword).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ResetPasswordRequest.ConfirmPassword).GetInvalidValueErrorMessage(typeof(ResetPasswordRequest)))
                .Equal(x => x.NewPassword)
                .WithMessage("رمز عبور جدید و تکرار آن یکسان نیستند");

        




        }
    }
}