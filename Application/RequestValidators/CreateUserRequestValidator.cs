
using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;
using ManageUsers.Domain;

namespace ManageUsers.Application.RequestValidators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .SetValidator(validator: new IgnoreCharacterValidator<CreateUserRequest>())
                .WithMessage(nameof(CreateUserRequest.FirstName).GetInvalidValueErrorMessage(typeof(CreateUserRequest)))
                .MaximumLength(StaticDetail.NameMaxLength)
                .WithMessage(nameof(CreateUserRequest.FirstName).GetMaxLengthErrorMessage(typeof(CreateUserRequest), StaticDetail.NameMaxLength));

            RuleFor(x => x.LastName)
               .SetValidator(validator: new IgnoreCharacterValidator<CreateUserRequest>())
               .WithMessage(nameof(CreateUserRequest.LastName).GetInvalidValueErrorMessage(typeof(CreateUserRequest)))
               .MaximumLength(StaticDetail.LastNameMaxLength)
               .WithMessage(nameof(CreateUserRequest.LastName).GetMaxLengthErrorMessage(typeof(CreateUserRequest), StaticDetail.LastNameMaxLength));

            RuleFor(o => o.PhoneNumber)
               .SetValidator(validator: new PhoneNumberValidator<CreateUserRequest>())
               .WithMessage(nameof(CreateUserRequest.PhoneNumber).GetInvalidValueErrorMessage(typeof(CreateUserRequest)));


            RuleFor(o => o.NationalCode)
              .SetValidator(validator: new NationalCodeValidator<CreateUserRequest>())
              .WithMessage(nameof(CreateUserRequest.NationalCode).GetInvalidValueErrorMessage(typeof(CreateUserRequest)));


            RuleFor(o => o.Email)
             .SetValidator(validator: new EmailValidator<CreateUserRequest>()!)
             .When(o => !string.IsNullOrEmpty(o.Email))
             .WithMessage(nameof(CreateUserRequest.Email).GetInvalidValueErrorMessage(typeof(CreateUserRequest)));

            RuleFor(x => x.UserName)
               .NotNull()
               .WithMessage(nameof(CreateUserRequest.UserName).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
               .NotEmpty()
               .WithMessage(nameof(CreateUserRequest.UserName).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)));

            RuleFor(x => x.Password)
               .NotNull()
               .WithMessage(nameof(CreateUserRequest.Password).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
               .NotEmpty()
               .WithMessage(nameof(CreateUserRequest.Password).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)));

            RuleFor(o => o.Password)
               .SetValidator(validator: new PasswordValidator<CreateUserRequest>())
               .WithMessage(nameof(CreateUserRequest.Password).GetInvalidValueErrorMessage(typeof(CreateUserRequest)));

            RuleFor(x => x.PostalCode)
              .MaximumLength(StaticDetail.PostalCodeLength)
              .WithMessage(nameof(CreateUserRequest.PostalCode).GetInvalidValueErrorMessage(typeof(CreateUserRequest)));

            RuleFor(o => o.UserRoleId)
                .NotNull()
                .WithMessage(nameof(CreateUserRequest.UserRoleId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .WithMessage(nameof(CreateUserRequest.UserRoleId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)));

            RuleFor(x => x.AreaId)
                .NotNull()
                .When(x => x.AccessLevel == AccessLevel.Area)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .When(x => x.AccessLevel == AccessLevel.Area)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .GreaterThan(0)
                .When(x => x.AccessLevel == AccessLevel.Area)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetGreaterThanErrorMessage(typeof(CreateUserRequest), 0))
                .Null()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .Empty()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)));


            RuleFor(x => x.ZoneId)
                .NotNull()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(CreateUserRequest.ZoneId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(CreateUserRequest.ZoneId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .GreaterThan(0)
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(CreateUserRequest.ZoneId).GetGreaterThanErrorMessage(typeof(CreateUserRequest), 0));

            RuleFor(x => x.AreaId)
                .Null()
                .When(x => x.AccessLevel == AccessLevel.Setad)
                .WithMessage("برای سطح دسترسی ستاد نباید منطقه انتخاب شود.");

            RuleFor(x => x.ZoneId)
                .Null()
                .When(x => x.AccessLevel == AccessLevel.Setad)
                .WithMessage("برای سطح دسترسی ستاد نباید ناحیه انتخاب شود.");

            RuleFor(x => x.SetadName)
                .NotNull()
                .When(x => x.AccessLevel == AccessLevel.Setad)
                .WithMessage("نام ستاد نمی تواند خالی باشد وقتی سطح دسترسی ستاد است.!")
                .NotEmpty()
                .When(x => x.AccessLevel == AccessLevel.Setad)
                .WithMessage("نام ستاد نمی تواند خالی باشد وقتی سطح دسترسی ستاد است.!");


            RuleFor(x => x.BirthDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.BirthDate.HasValue)
                .WithMessage("تاریخ تولد نمی‌تواند در آینده باشد.");
        }
    }
}