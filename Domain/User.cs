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
    public string? AvatarUrl { get; set; }
    public bool Enabled { get; set; } = true;
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Setad;
    public string? CreatedById { get; set; }

    public string? OTPCode { get; set; }

    public DateTime? SendDateTimeOTPCode { get; set; }

    public bool IsFirstLogin { get; set; } = true;
    public DateTime? PasswordExpiresAt { get; set; }
    public DateTime? BirthDate { get; set; }

    public string? SetadName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public int? AreaId { get; set; }
    public Area? Area { get; set; }

    public int? ZonId { get; set; }
    public Zone? Zone { get; set; }

    public ApplicationUserToken? ApplicationUserToken { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

