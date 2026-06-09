using Microsoft.EntityFrameworkCore;
using TapAi.Module.Catalog.Domain.Entity;
using TapAi.Shared.Application.Context;

namespace TapAi.Module.Catalog.Persistence.Contexts;

/// <summary>
/// <see cref="CommandDbContext"/> (yazma) və <see cref="QueryDbContext"/> (oxuma)
/// tərəfindən paylaşılan baza EF Core konteksti.
/// <see cref="ICatalogDbSets"/>-i implement edir ki, hər iki konkret kontekst
/// property elanlarını təkrarlamadan oxuma interfeysini ödəsin.
/// Explicit interfeys üzvləri <c>DbSet&lt;T&gt;</c>-ni <see cref="IQueryable{T}"/> kimi
/// təqdim edir və beləcə EF mutasiya səthini interfeysdən çıxarır.
/// </summary>
public abstract class CatalogDbContext(DbContextOptions options)
    : AppDbContext(options), ICatalogDbSets
{
    public DbSet<Car> Cars { get; set; } = null!;
    public DbSet<CarDraft> CarDrafts { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Model> Models { get; set; } = null!;

    // ── ICatalogDbSets explicit implementasiyaları ──────────────────────
    IQueryable<Car> ICatalogDbSets.Cars => Cars;
    IQueryable<CarDraft> ICatalogDbSets.CarDrafts => CarDrafts;
    IQueryable<Brand> ICatalogDbSets.Brands => Brands;
    IQueryable<Model> ICatalogDbSets.Models => Models;
}