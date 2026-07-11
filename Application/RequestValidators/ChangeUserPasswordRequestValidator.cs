using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators
{
    public class ChangeUserPasswordRequestValidator : AbstractValidator<ChangeUserPasswordRequest>
    {
        public ChangeUserPasswordRequestValidator()
        {
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(nameof(ChangeUserPasswordRequest.ConfirmNewPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ChangeUserPasswordRequest.ConfirmNewPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)))
                .Equal(x => x.NewPassword)
                .WithMessage("رمز عبور جدید و تکرار آن یکسان نیستند");

            RuleFor(o => o.CurrentPassword)
                .SetValidator(validator: new PasswordValidator<ChangeUserPasswordRequest>())
                .WithMessage(nameof(ChangeUserPasswordRequest.CurrentPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)));

            RuleFor(o => o.NewPassword)
               .SetValidator(validator: new PasswordValidator<ChangeUserPasswordRequest>())
               .WithMessage(nameof(ChangeUserPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)));

            RuleFor(o => o.NewPassword)
                .NotEmpty()
                .WithMessage(nameof(ChangeUserPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)))
                .NotNull()
                .WithMessage(nameof(ChangeUserPasswordRequest.NewPassword).GetInvalidValueErrorMessage(typeof(ChangeUserPasswordRequest)))
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("رمز عبور جدید و قبلی یکسان هستند!");

        }
    }
}