using TapAi.Module.Catalog.Domain.Entity;

namespace TapAi.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Catalog verilənlər bazasının təqdim etdiyi entity set-lərini elan edir.
/// <see cref="ICatalogReadDbContext"/> (oxuma handler-ləri) tərəfindən istifadə olunur və
/// <see cref="CatalogDbContext"/> üzərində EF <c>DbSet</c> property-ləri ilə explicit implement edilir.
/// <para>
/// <c>DbSet&lt;T&gt;</c> əvəzinə <see cref="IQueryable{T}"/> istifadəsi interfeysi
/// EF Core mutasiya səthindən (<c>Add</c>, <c>Remove</c>, …) azad saxlayır.
/// Əlaqəli entity oxumaq lazım olan command handler-lər <see cref="ICatalogWriteDbContext"/>
/// ilə yanaşı <see cref="ICatalogReadDbContext"/>-i də inject etməlidir.
/// </para>
/// </summary>
public interface ICatalogDbSets
{
    IQueryable<Car> Cars { get; }
    IQueryable<CarDraft> CarDrafts { get; }
    IQueryable<Brand> Brands { get; }
    IQueryable<Model> Models { get; }
}