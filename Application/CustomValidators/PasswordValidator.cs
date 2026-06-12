using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ManageUsers.Application.CustomValidators;

public class PasswordValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "PasswordValidator";

    // Compiling the regex pattern once to improve performance
    private static readonly Regex NewPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])[A-Za-z0-9!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]{8,64}$", RegexOptions.Compiled);

    public override bool IsValid(ValidationContext<T> context, string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        return NewPasswordRegex.IsMatch(password);
    }
}