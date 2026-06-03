using Microsoft.EntityFrameworkCore;
using TapAi.Shared.Application.Context;

namespace TapAi.Module.Media.Persistence.Contexts;

public sealed class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : MediaDbContext(options), IMediaWriteDbContext
{
    // ── IWriteDbContext explicit implementations ─────────────────────────
    void IWriteDbContext.Attach<TEntity>(TEntity entity)
        => Attach(entity);

    void IWriteDbContext.AttachRange<TEntity>(IEnumerable<TEntity> entities)
        => AttachRange(entities);

    void IWriteDbContext.Add<TEntity>(TEntity entity)
        => Add(entity);

    void IWriteDbContext.AddRange<TEntity>(IEnumerable<TEntity> entities)
        => AddRange(entities);

    void IWriteDbContext.Remove<TEntity>(TEntity entity)
        => Remove(entity);
}