using Microsoft.EntityFrameworkCore;

namespace TapAi.Module.Catalog.Persistence.Contexts;

public sealed class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : CatalogDbContext(options), ICatalogReadDbContext
{
    /// <summary>
    /// Query DB yalnız oxunaqlıdır — yazılar <see cref="CommandDbContext"/> üzərindən getməlidir.
    /// </summary>
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException(
            "QueryDbContext is read-only. Use CommandDbContext to persist changes.");

    /// <inheritdoc cref="SaveChangesAsync"/>
    public override int SaveChanges() =>
        throw new InvalidOperationException(
            "QueryDbContext is read-only. Use CommandDbContext to persist changes.");
}