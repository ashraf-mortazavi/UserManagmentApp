namespace ManageUsers.Application.DTOs
{
    public sealed record SetPasswordRequest(
        string UserName,
        string Password
    );

    public sealed record SetPasswordResponse(
        bool Success,
        string Message
    );


}
