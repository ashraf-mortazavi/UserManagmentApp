namespace ManageUsers.Application.DTOs
{
    public class GetRolePermissionsResponse
    {
        public List<RolePermissionsItem>? Items { get; set; }
        public string? FailedResult { get; set; }
    }


    public sealed record RolePermissionsItem(
        List<RoleDto> Roles,
        List<PermissionDto> Permissions);

    public sealed record PermissionDto(int Id, string Key, string Name, bool IsActive, int SortOrder, int? ParentId);
    public sealed record RoleDto(int Id, string Name);
}
