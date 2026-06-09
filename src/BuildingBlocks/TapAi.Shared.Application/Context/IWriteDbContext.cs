namespace TapAi.Shared.Application.Context;

/// <summary>
/// Yazma (command) verilənlər bazası kontekstləri üçün marker interfeysi.
/// Bilərəkdən <b>heç bir</b> sorğu səthi təqdim etmir (<c>Set&lt;T&gt;</c> yoxdur) —
/// belə ki, bu interfeys üzərindən oxumağa çalışan command handler kompilyasiya
/// olunmayacaq. Bütün oxumalar <see cref="IReadDbContext"/> üzərindən getməlidir.
/// <para>
/// Load-to-modify (yüklə-dəyiş) nümunəsi:
/// <code>
/// var entity = await readDb.Entities.AsNoTracking().FirstOrDefaultAsync(...);
/// writeDb.Attach(entity);   // yazma tərəfindəki change tracker-ə tanıt
/// entity.DoSomething();     // domain metodu ilə dəyiş
/// await writeDb.SaveChangesAsync(ct);
/// </code>
/// </para>
/// </summary>
public interface IWriteDbContext
{
    // ── Saxlama (persistence) ───────────────────────────────────────────────
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // ── Attach (yüklə-dəyiş) ──────────────────────────────────────────────────
    /// <summary>
    /// Track olunmadan yüklənmiş entity-ni (məs. <see cref="IReadDbContext"/>-dən
    /// <c>AsNoTracking()</c> ilə) yazma tərəfindəki change tracker-ə əlavə edir.
    /// Entity <c>Unchanged</c> vəziyyətinə keçir; sonrakı property dəyişiklikləri
    /// <see cref="SaveChangesAsync"/>-dən əvvəl avtomatik aşkarlanır.
    /// </summary>
    void Attach<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>Track olunmamış bir neçə entity-ni yazma tərəfindəki change tracker-ə əlavə edir.</summary>
    void AttachRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    // ── Add ─────────────────────────────────────────────────────────────────
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    // ── Remove ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Entity-ni silinmək üçün işarələyir. Əgər entity hələ track olunmayıbsa,
    /// əvvəlcə <c>Deleted</c> vəziyyətində attach edilir; ona görə də bu metoddan
    /// əvvəl onu <see cref="IReadDbContext"/> üzərindən yükləmək kifayətdir.
    /// </summary>
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}