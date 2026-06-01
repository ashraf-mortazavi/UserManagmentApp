using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
namespace ManageUsers.Domain;

public sealed class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

