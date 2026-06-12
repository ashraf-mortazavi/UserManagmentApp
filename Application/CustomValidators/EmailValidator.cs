using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ManageUsers.Application.CustomValidators;


public class EmailValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "EmailValidator";

    // Simple and practical email pattern (compiled for performance)
    private static readonly Regex EmailRegex = new(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled);

    public override bool IsValid(ValidationContext<T> context, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    public bool IsValid(string email)
    {
        return IsValid(null, email);
    }
}