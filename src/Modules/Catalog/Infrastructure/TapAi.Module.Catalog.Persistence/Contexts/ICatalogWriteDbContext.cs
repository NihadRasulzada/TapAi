using TapAi.Shared.Application.Context;

namespace TapAi.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Catalog verilənlər bazası üçün yazma konteksti.
/// Yalnız <see cref="IWriteDbContext"/>-dən gələn ümumi mutasiya köməkçilərini təqdim edir
/// — entity-set property-ləri yoxdur — beləcə command handler-lər dəyişdirmədən əvvəl
/// əlaqəli entity oxumaq lazım olduqda <see cref="ICatalogReadDbContext"/>-i inject etməyə məcbur olur.
/// </summary>
public interface ICatalogWriteDbContext : IWriteDbContext { }