using FluentValidation;
using ManageUsers.Application.Common.Utilities;
using ManageUsers.Application.CustomValidators;
using ManageUsers.Application.DTOs;
using ManageUsers.Application.Extentions;

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

            RuleFor(o => o.UserRoleIds)
                .NotNull()
                .WithMessage(nameof(CreateUserRequest.UserRoleIds).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .WithMessage(nameof(CreateUserRequest.UserRoleIds).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)));


            RuleFor(x => x.AreaId)
                .NotNull()
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .GreaterThan(0)
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.AreaId).GetGreaterThanErrorMessage(typeof(CreateUserRequest), 0));


            RuleFor(x => x.RegionId)
                .NotNull()
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.RegionId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .NotEmpty()
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.RegionId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
                .GreaterThan(0)
                .When(x => !x.OrganizationId.HasValue)
                .WithMessage(nameof(CreateUserRequest.RegionId).GetGreaterThanErrorMessage(typeof(CreateUserRequest), 0));

            RuleFor(x => x.OrganizationId)
               .NotNull()
               .When(x => !x.AreaId.HasValue && !x.RegionId.HasValue)
               .WithMessage(nameof(CreateUserRequest.OrganizationId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
               .NotEmpty()
               .When(x => !x.AreaId.HasValue && !x.RegionId.HasValue)
               .WithMessage(nameof(CreateUserRequest.OrganizationId).GetNullOrEmptyErrorMessage(typeof(CreateUserRequest)))
               .GreaterThan(0)
               .When(x => !x.AreaId.HasValue && !x.RegionId.HasValue)
               .WithMessage(nameof(CreateUserRequest.OrganizationId).GetGreaterThanErrorMessage(typeof(CreateUserRequest), 0));
        }
    }
}