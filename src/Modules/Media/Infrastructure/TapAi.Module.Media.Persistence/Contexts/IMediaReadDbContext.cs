using TapAi.Shared.Application.Context;

namespace TapAi.Module.Media.Persistence.Contexts;

/// <summary>
/// Media verilənlər bazasının yalnız oxuma görünüşü.
/// Bunu query handler-lərində inject edin — <c>SaveChangesAsync</c> mövcud deyil.
/// </summary>
public interface IMediaReadDbContext : IReadDbContext, IMediaDbSets { }