using System.Net;
using System.Text;
using System.Text.Json;

namespace ManageUsers.APIs.Extentions;

public static class CustomExceptionValidateFilter
{
    public static void ResponseHandling(this IApplicationBuilder app, ILogger logger)
    {
        app.Use(async (context, next) =>
        {
            await next();

            switch (context.Response.StatusCode)
            {
                case (int)HttpStatusCode.Unauthorized:
                    await WriteResponseAsync(context: context, httpStatusCode: HttpStatusCode.Unauthorized, errorMessages: "عدم احراز هویت");
                    break;
                case (int)HttpStatusCode.Forbidden:
                    await WriteResponseAsync(context: context, httpStatusCode: HttpStatusCode.Forbidden, errorMessages: "عدم دسترسی");
                    break;
                case (int)HttpStatusCode.NotAcceptable:
                    await WriteResponseAsync(context: context, httpStatusCode: HttpStatusCode.NotAcceptable, errorMessages: "Need To Refresh Token");
                    break;
                case (int)HttpStatusCode.MethodNotAllowed:
                    await WriteResponseAsync(context: context, httpStatusCode: HttpStatusCode.MethodNotAllowed, errorMessages: "متد درخواست اشتباه است.");
                    break;
                default:
                    if (context.GetEndpoint() is null && context.Response.StatusCode != (int)HttpStatusCode.BadRequest && context.Response.StatusCode != (int)HttpStatusCode.BadGateway)
                    {
                        await WriteResponseAsync(context: context, httpStatusCode: HttpStatusCode.NotFound, errorMessages: "آدرس اشتباه");
                    }
                    break;
            }
        });
    }


    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode httpStatusCode, string errorMessages)
    {
        context.Response.ContentType = "application/json";
        string responseObject = string.Empty;
        byte[] exceptionData = [];
        context.Response.StatusCode = (int)httpStatusCode;
        responseObject = JsonSerializer.Serialize(new APIResponse<object>
        {
            StatusCode = httpStatusCode,
            ErrorMessage = [errorMessages],
            IsSuccess = false,
        });
        exceptionData = Encoding.UTF8.GetBytes(responseObject);
        await context.Response.Body.WriteAsync(exceptionData, 0, exceptionData.Length, CancellationToken.None);
    }
}
