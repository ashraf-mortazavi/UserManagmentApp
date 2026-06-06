using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
namespace ManageUsers.Domain;

public sealed class Role : IdentityRole<int>
{
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public int? NextLowerRoleId { get; set; }
    public Role? NextLowerRole { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

