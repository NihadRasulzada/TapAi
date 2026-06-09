using Microsoft.EntityFrameworkCore;
using TapAi.Shared.Application.Context;
using MediaEntity = TapAi.Module.Media.Domain.Entity.Media;

namespace TapAi.Module.Media.Persistence.Contexts;

/// <summary>
/// <see cref="CommandDbContext"/> (yazma) və <see cref="QueryDbContext"/> (oxuma)
/// tərəfindən paylaşılan baza EF Core konteksti.
/// <see cref="IMediaDbSets"/>-i implement edir ki, hər iki konkret kontekst
/// property elanlarını təkrarlamadan oxuma interfeysini ödəsin.
/// Explicit interfeys üzvü <c>DbSet&lt;Media&gt;</c>-ni <see cref="IQueryable{T}"/> kimi
/// təqdim edir və beləcə EF mutasiya səthini interfeysdən çıxarır.
/// </summary>
public abstract class MediaDbContext(DbContextOptions options) : AppDbContext(options), IMediaDbSets
{
    public DbSet<MediaEntity> Medias { get; set; } = null!;

    // ── IMediaDbSets explicit implementasiyası ──────────────────────────
    IQueryable<MediaEntity> IMediaDbSets.Medias => Medias;
}