namespace ManageUsers.Domain;

public sealed class Permission : BaseEntity
{
    public string Key { get; set; } = null!;   // e.g. menu.users.view
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;


    public int? ParentId { get; set; }
    public Permission? Parent { get; set; }
    public ICollection<Permission> Children { get; set; } = new List<Permission>();

    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
