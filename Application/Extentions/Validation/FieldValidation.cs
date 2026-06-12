using ProductivityTools.DescriptionValue;

using Microsoft.AspNetCore.Http;

namespace ManageUsers.Application.Extentions;

public static class FieldValidation
{
    public static string GetNullOrEmptyErrorMessage(this string propertyName, Type type)
    {
        return $@"فیلد '{type.GetPropertyDescription(propertyName)}' نمی تواند خالی باشد!";
    }

    public static string GetInvalidValueErrorMessage(this string propertyName, Type type)
    {
        return $@"مقدار فیلد '{type.GetPropertyDescription(propertyName)}' معتبر نمی باشد!";
    }


    public static string GetMinLengthErrorMessage(this string propertyName, Type type, object MinLength)
    {
        return $@"مقدار فیلد '{type.GetPropertyDescription(propertyName)}' نمی تواند کمتر از {MinLength} کاراکتر باشد!";
    }

    public static string GetMaxLengthErrorMessage(this string propertyName, Type type, object MaxLength)
    {
        return $@"مقدار فیلد '{type.GetPropertyDescription(propertyName)}' نمی تواند بیشتر از {MaxLength} کاراکتر باشد!";
    }


    public static string GetExactLengthErrorMessage(this string propertyName, Type type, object Length)
    {
        return $@"مقدار فیلد '{type.GetPropertyDescription(propertyName)}' باید {Length} کاراکتر باشد!";
    }

    public static string GetGreaterThanErrorMessage(this string propertyName, Type type, int value)
    {
        return $@"مقدار فیلد '{type.GetPropertyDescription(propertyName)}' باید {value} بزرگتر از باشد!";

    }


}