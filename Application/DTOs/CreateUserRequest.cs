
namespace ManageUsers.Application.DTOs
{
    public sealed record CreateUserRequest(string FirstName, string LastName, string PhoneNumber,
        string NationalCode, string Email, string PostalCode, string UserName, string Password, List<string> UserRoles)
    {
    };

    public sealed record CreateUserResponse(
        Guid Id
    );

}