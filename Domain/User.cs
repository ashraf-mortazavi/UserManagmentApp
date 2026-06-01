using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ManageUsers.Domain;

public sealed class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;
    public string? PostalCode { get; set; }
    public bool Enabled { get; set; } = true;
    public Guid? CreatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ApplicationUserToken? ApplicationUserToken { get; set; }
}

