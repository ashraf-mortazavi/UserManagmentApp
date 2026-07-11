
using FluentValidation;
using FluentValidation.Validators;

namespace ManageUsers.Application.CustomValidators;

public class NationalCodeValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "NationalCodeValidator";

    public override bool IsValid(ValidationContext<T> context, string Character)
    {
        if (string.IsNullOrWhiteSpace(Character))
            return false;

        if(Character.Length != 10)
            return false;

        switch (Character)
        {
            case "0000000000":
            case "1111111111":
            case "22222222222":
            case "33333333333":
            case "4444444444":
            case "5555555555":
            case "6666666666":
            case "7777777777":
            case "8888888888":
            case "9999999999":
                return false;
        }

        //char[] chArray = Character.ToCharArray();
        //int[] melicode = new int[chArray.Length];
        //for (int i = 0; i < chArray.Length; i++)
        //{
        //    melicode[i] = (int)char.GetNumericValue(chArray[i]);
        //}

        //int total = (melicode[0] * 10) + (melicode[1] * 9) + (melicode[2] * 8) + (melicode[3] * 7) + (melicode[4] * 6) + (melicode[5] * 5) + (melicode[6] * 4) + (melicode[7] * 3) + (melicode[8] * 2);
        //int mode = total % 11;
        //if (mode >= 2)
        //{
        //    if (melicode[9] == 11 - mode)
        //    {
        //        return true;
        //    }
        //}
        //else
        //{
        //    if (melicode[9] == mode)
        //    {
        //        return true;
        //    }
        //}
        return true;
    }
}