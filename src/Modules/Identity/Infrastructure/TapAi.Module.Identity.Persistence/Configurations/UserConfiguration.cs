using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TapAi.Module.Identity.Domain.Entity;
using TapAi.Shared.Infrastructure.EntityConfiguration;

namespace TapAi.Module.Identity.Persistence.Configurations;

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.PhoneNumber)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(u => u.NormalizedPhoneNumber)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.HasIndex(u => u.NormalizedPhoneNumber)
            .IsUnique()
            .HasFilter("\"NormalizedPhoneNumber\" IS NOT NULL");

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.IsAdmin).IsRequired();
        builder.Property(u => u.IsBlocked).IsRequired();
        builder.Property(u => u.FailedLoginCount).IsRequired();

        builder.Property(u => u.BlockedUntilSeconds)
            .IsRequired(false);

        builder.HasMany<RefreshToken>()
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
