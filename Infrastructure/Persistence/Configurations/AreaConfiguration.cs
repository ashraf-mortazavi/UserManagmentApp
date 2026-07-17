using ManageUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageUsers.Infrastructure.Persistence.Configurations;


public sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(700);

        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);


        builder.HasOne(x => x.Zone)
          .WithMany(x => x.Areas)
          .HasForeignKey(x => x.ZoneId)
          .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.Id, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

 
    }
}
