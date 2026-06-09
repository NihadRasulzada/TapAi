using TapAi.Shared.Application.Context;

namespace TapAi.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Catalog verilənlər bazasının yalnız oxuma görünüşü.
/// Bunu query handler-lərində və əlaqəli entity-ni yazma tərəfindəki change tracker-ə
/// yükləmədən oxumaq lazım olan command handler-lərində inject edin.
/// </summary>
public interface ICatalogReadDbContext : IReadDbContext, ICatalogDbSets { }