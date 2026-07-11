namespace ManageUsers.Application.DTOs
{
    public class GetAllRolesResponse :BaseResponse
    {
        public List<RoleResponse> Roles { get; set; } = new();
    }
    public class RoleResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
