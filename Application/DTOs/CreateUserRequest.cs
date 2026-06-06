
namespace ManageUsers.Application.DTOs
{
    public sealed record CreateUserRequest(string FirstName, string LastName, string PhoneNumber,
        string NationalCode, string? Email, string? PostalCode, string UserName, string Password,
        string? PersonalCode, string? Position, string? Description, 
        int? OrganizationId, int? AreaId,int? RegionId,
        List<string> PermissionsIds, List<int> UserRoleIds)
    {
    };

    public sealed record CreateUserResponse(
        string Id
    );

}