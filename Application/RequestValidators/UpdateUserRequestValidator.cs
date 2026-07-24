using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;
using ManageUsers.Domain;

namespace ManageUsers.Application.RequestValidators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .SetValidator(validator: new IgnoreCharacterValidator<UpdateUserRequest>())
                .WithMessage(nameof(UpdateUserRequest.FirstName).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)))
                .MaximumLength(StaticDetail.NameMaxLength)
                .WithMessage(nameof(UpdateUserRequest.FirstName).GetMaxLengthErrorMessage(typeof(UpdateUserRequest), StaticDetail.NameMaxLength));

            RuleFor(x => x.LastName)
                .SetValidator(validator: new IgnoreCharacterValidator<UpdateUserRequest>())
                .WithMessage(nameof(UpdateUserRequest.LastName).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)))
                .MaximumLength(StaticDetail.LastNameMaxLength)
                .WithMessage(nameof(UpdateUserRequest.LastName).GetMaxLengthErrorMessage(typeof(UpdateUserRequest), StaticDetail.LastNameMaxLength));

            RuleFor(o => o.PhoneNumber)
                .SetValidator(validator: new PhoneNumberValidator<UpdateUserRequest>())
                .WithMessage(nameof(UpdateUserRequest.PhoneNumber).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)));

            RuleFor(o => o.NationalCode)
                .SetValidator(validator: new NationalCodeValidator<UpdateUserRequest>())
                .WithMessage(nameof(UpdateUserRequest.NationalCode).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)));

            RuleFor(o => o.Email)
                .SetValidator(validator: new EmailValidator<UpdateUserRequest>()!)
                .When(o => !string.IsNullOrEmpty(o.Email))
                .WithMessage(nameof(UpdateUserRequest.Email).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)));

            RuleFor(x => x.PostalCode)
                .MaximumLength(StaticDetail.PostalCodeLength)
                .WithMessage(nameof(UpdateUserRequest.PostalCode).GetInvalidValueErrorMessage(typeof(UpdateUserRequest)));

            RuleFor(o => o.RoleId)
                .NotNull()
                .WithMessage(nameof(UpdateUserRequest.RoleId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .NotEmpty()
                .WithMessage(nameof(UpdateUserRequest.RoleId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)));

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
