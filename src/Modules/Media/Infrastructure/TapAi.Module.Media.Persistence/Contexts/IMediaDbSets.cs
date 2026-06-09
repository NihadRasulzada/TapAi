using MediaEntity = TapAi.Module.Media.Domain.Entity.Media;

namespace TapAi.Module.Media.Persistence.Contexts;

/// <summary>
/// Media verilənlər bazasının təqdim etdiyi entity set-lərini elan edir.
/// <see cref="IMediaReadDbContext"/> (oxuma tərəfi) tərəfindən istifadə olunur və
/// <see cref="MediaDbContext"/> üzərində EF <c>DbSet</c> property-si ilə explicit implement edilir.
/// </summary>
public interface IMediaDbSets
{
    IQueryable<MediaEntity> Medias { get; }
}