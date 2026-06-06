namespace ManageUsers.Application.DTOs
{
    public class GetRolesResponse
    {
        public List<RoleItem>? Items { get; set; }
        public string? FailedResult { get; set; }
    }

    public sealed record RoleItem(int Id, string Name, string NextRoleName);
}
