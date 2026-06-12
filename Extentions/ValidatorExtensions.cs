using ManageUsers.Filters;

namespace ManageUsers.Extentions;

internal static class ValidatorExtensions
{
   public static RouteHandlerBuilder Validator<T>(this RouteHandlerBuilder handlerBuilder)
   where T : class
    {
        handlerBuilder.AddEndpointFilter<EndpointValidatorFilter<T>>();
        return handlerBuilder;
    }
}

