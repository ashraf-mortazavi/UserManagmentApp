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

            RuleFor(o => o.UserRoleIds)
                .NotNull()
                .WithMessage(nameof(UpdateUserRequest.UserRoleIds).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .NotEmpty()
                .WithMessage(nameof(UpdateUserRequest.UserRoleIds).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)));

            RuleFor(x => x.AreaId)
                .NotNull()
                .When(x => x.AccessLevel == AccessLevel.Area || x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .NotEmpty()
                .When(x => x.AccessLevel == AccessLevel.Area || x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .GreaterThan(0)
                .When(x => x.AccessLevel == AccessLevel.Area || x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.AreaId).GetGreaterThanErrorMessage(typeof(UpdateUserRequest), 0));

            RuleFor(x => x.RegionId)
                .NotNull()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.RegionId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .NotEmpty()
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.RegionId).GetNullOrEmptyErrorMessage(typeof(UpdateUserRequest)))
                .GreaterThan(0)
                .When(x => x.AccessLevel == AccessLevel.Zone)
                .WithMessage(nameof(UpdateUserRequest.RegionId).GetGreaterThanErrorMessage(typeof(UpdateUserRequest), 0));

            RuleFor(x => x.AreaId)
                .Null()
                .When(x => x.AccessLevel == AccessLevel.Setad)
                .WithMessage("برای سطح دسترسی ستاد نباید منطقه انتخاب شود.");

            RuleFor(x => x.RegionId)
                .Null()
                .When(x => x.AccessLevel != AccessLevel.Zone)
                .WithMessage("ناحیه فقط برای سطح دسترسی ناحیه قابل انتخاب است.");
        }
    }
}
