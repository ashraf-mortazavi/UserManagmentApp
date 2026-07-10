using System.Net;
using System.Text;
using System.Text.Json;

namespace ManageUsers.APIs.Extentions;

public static class CustomExceptionValidateFilter
{
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
