using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace ManageUsers.Filters;

public class EndpointValidatorFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    private IValidator<T> Validator => validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? inputData = context.GetArgument<T>(0);

        if (inputData is not null)
        {
            ValidationResult validationResult = await Validator.ValidateAsync(inputData);
            if (!validationResult.IsValid)
            {
                List<string> errors = [];
                foreach (var item in validationResult.ToDictionary())
                {
                    errors.Add(item.Value[0]);
                }
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                var responseObject = new APIResponse<object>()
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    ErrorMessage = errors,
                    IsSuccess = false,
                };

                return responseObject;
            }
        }

        return await next.Invoke(context);
    }
}
