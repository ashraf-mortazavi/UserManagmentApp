using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ManageUsers.Domain;

public sealed class User : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;
    public string? PostalCode { get; set; }
    public string? PersonalCode { get; set; }
    public string? Position { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    public string? CreatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public int? AreaId { get; set; }
    public Area? Area { get; set; }

    public int? RegionId { get; set; }
    public Region? Region { get; set; }

    public ApplicationUserToken? ApplicationUserToken { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

