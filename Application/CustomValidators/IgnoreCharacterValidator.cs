using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ManageUsers.Application.CustomValidators;

public class IgnoreCharacterValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "IgnoreCharacterValidator";

    // Compiling the regex pattern once to improve performance
    private static readonly Regex IgnoreRegex = new Regex(@"^[^\\/:*;\.\)\(']+$", RegexOptions.Compiled);

    public override bool IsValid(ValidationContext<T> context, string character)
    {
        if (string.IsNullOrEmpty(character))
            return false;

        return IgnoreRegex.IsMatch(character);
    }
    public bool IsValid(string Character)
    {
        return IsValid(null, Character);
    }
}