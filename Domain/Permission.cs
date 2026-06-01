namespace ManageUsers.Domain;

public sealed class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Key { get; set; } = null!;   // e.g. menu.users.view
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; } 

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
