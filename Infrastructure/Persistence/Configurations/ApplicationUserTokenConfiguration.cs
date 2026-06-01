
using ManageUsers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageUsers.Infrastructure.Persistence;

public sealed class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
{
    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
    {

        builder.HasKey(x => x.HashToken);

        builder.Property(x => x.TokenExpirationDate)
            .IsRequired(true);
            

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RefreshTokenExpirationDate)
          .IsRequired(true);


        builder.Property(x => x.PhoneNumber)
           .IsRequired(false)
           .HasMaxLength(11);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
        .WithOne(u => u.ApplicationUserToken)
        .HasForeignKey<ApplicationUserToken>(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired(false);
    }
}
