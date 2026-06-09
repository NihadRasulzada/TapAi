using TapAi.Module.Identity.Domain.Entity;

namespace TapAi.Module.Identity.Persistence.Contexts;

/// <summary>
/// Identity verilənlər bazasının təqdim etdiyi entity set-lərini elan edir.
/// <see cref="IIdentityReadDbContext"/> (oxuma handler-ləri) tərəfindən istifadə olunur və
/// <see cref="IdentityDbContext"/> üzərində EF DbSet property-ləri ilə explicit implement edilir.
/// DbSet əvəzinə <see cref="IQueryable{T}"/> istifadəsi interfeysi
/// EF Core mutasiya səthindən azad saxlayır.
/// </summary>
public interface IIdentityDbSets
{
    IQueryable<User> Users { get; }
    IQueryable<RefreshToken> RefreshTokens { get; }
}
