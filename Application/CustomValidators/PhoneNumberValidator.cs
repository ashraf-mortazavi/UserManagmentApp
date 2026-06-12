using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ManageUsers.Application.CustomValidators;

public class PhoneNumberValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "PhoneNumberValidator";

    // Compiling the regex pattern once to improve performance
    private static readonly Regex PhoneNumberRegex = new(@"^(?:0)?(9\d{9})$", RegexOptions.Compiled);


    public override bool IsValid(ValidationContext<T> context, string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        return PhoneNumberRegex.IsMatch(phoneNumber);
    }
}