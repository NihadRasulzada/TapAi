using TapAi.Shared.Application.Context;

namespace TapAi.Module.Identity.Persistence.Contexts;

/// <summary>
/// Identity verilənlər bazasının yalnız oxuma görünüşü.
/// Bunu query handler-lərində və entity-ni yazma tərəfindəki change tracker-ə
/// yükləmədən oxumaq lazım olan command handler-lərində inject edin.
/// </summary>
public interface IIdentityReadDbContext : IReadDbContext, IIdentityDbSets { }
