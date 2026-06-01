using ManageUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ManageUsers.Infrastructure.Persistence;
public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        //builder.ToTable("roles");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Key).IsUnique();

        builder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(180)
            .IsRequired();


        builder.Property(x => x.CreatedAt)
            .IsRequired(false);
    }
}
