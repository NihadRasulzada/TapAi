using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Domain.Entity;
using TapAi.Shared.Application.Context;

namespace TapAi.Module.Identity.Persistence.Contexts;

/// <summary>
/// <see cref="CommandDbContext"/> (yazma) və <see cref="QueryDbContext"/> (oxuma)
/// tərəfindən paylaşılan baza EF Core konteksti.
/// <see cref="IIdentityDbSets"/>-i implement edir ki, hər iki konkret kontekst
/// property elanlarını təkrarlamadan oxuma interfeysini ödəsin.
/// </summary>
public abstract class IdentityDbContext(DbContextOptions options)
    : AppDbContext(options), IIdentityDbSets
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    // ── IIdentityDbSets explicit implementasiyaları ──────────────────────────
    IQueryable<User> IIdentityDbSets.Users => Users;
    IQueryable<RefreshToken> IIdentityDbSets.RefreshTokens => RefreshTokens;
}
