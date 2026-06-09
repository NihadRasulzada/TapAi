using TapAi.Shared.Application.Context;

namespace TapAi.Module.Media.Persistence.Contexts;

/// <summary>
/// Media verilənlər bazası üçün yazma konteksti.
/// Yalnız <see cref="IWriteDbContext"/>-dən gələn ümumi mutasiya köməkçilərini təqdim edir
/// — entity-set property-ləri yoxdur — beləcə consumer-lər track olunan toplu sorğular üçün
/// <c>Set&lt;Media&gt;()</c>, ayrı-ayrı mutasiyalar üçün isə tipli köməkçiləri istifadə edir.
/// </summary>
public interface IMediaWriteDbContext : IWriteDbContext { }