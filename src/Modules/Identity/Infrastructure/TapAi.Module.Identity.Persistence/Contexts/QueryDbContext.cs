using Microsoft.EntityFrameworkCore;

namespace TapAi.Module.Identity.Persistence.Contexts;

public sealed class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : IdentityDbContext(options), IIdentityReadDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(QueryDbContext).Assembly);
    }

    /// <summary>
    /// QueryDbContext yalnız oxunaqlıdır — yazılar <see cref="CommandDbContext"/> üzərindən getməlidir.
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
