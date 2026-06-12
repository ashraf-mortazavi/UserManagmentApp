using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

namespace ManageUsers.Application.RequestValidators
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.UserName)
               .NotNull()
               .WithMessage(nameof(LoginUserRequest.UserName).GetNullOrEmptyErrorMessage(typeof(LoginUserRequest)))
               .NotEmpty()
               .WithMessage(nameof(LoginUserRequest.UserName).GetNullOrEmptyErrorMessage(typeof(LoginUserRequest)));

            RuleFor(x => x.Password)
               .NotNull()
               .WithMessage(nameof(LoginUserRequest.Password).GetNullOrEmptyErrorMessage(typeof(LoginUserRequest)))
               .NotEmpty()
               .WithMessage(nameof(LoginUserRequest.Password).GetNullOrEmptyErrorMessage(typeof(LoginUserRequest)));

            RuleFor(x => x.Password)
               .SetValidator(validator: new PasswordValidator<LoginUserRequest>())
               .WithMessage(nameof(LoginUserRequest.Password).GetInvalidValueErrorMessage(typeof(LoginUserRequest)));

        }
    }
}