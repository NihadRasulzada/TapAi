using TapAi.Shared.Application.Context;

namespace TapAi.Module.Media.Persistence.Contexts;

/// <summary>
/// Read-only view of the Media database.
/// Inject this in query handlers — <c>SaveChangesAsync</c> is not available.
/// </summary>
public interface IMediaReadDbContext : IReadDbContext, IMediaDbSets { }