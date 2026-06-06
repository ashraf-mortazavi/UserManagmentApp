
using ManageUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageUsers.Infrastructure.Persistence;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.NationalCode).IsUnique();

        builder.Property(x => x.Email)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.NationalCode)
          .IsRequired(true)
          .HasMaxLength(10);


        builder.Property(x => x.PostalCode)
           .IsRequired(false)
           .HasMaxLength(10);

        builder.Property(x => x.PersonalCode)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(x => x.Position)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(700);

        builder.Property(x => x.UserName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.Enabled)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);


        builder.HasOne(x => x.Organization)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Area)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Region)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
