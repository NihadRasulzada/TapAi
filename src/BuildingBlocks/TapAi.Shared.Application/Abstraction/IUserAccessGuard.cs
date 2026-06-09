namespace TapAi.Shared.Application.Abstraction;

/// <summary>
/// Modullararası istifadəçi statusu yoxlaması. Implementasiya Identity modulunda
/// yerləşir; digər modullar yalnız bu interfeysə bağlıdır.
/// </summary>
public interface IUserAccessGuard
{
    /// <summary>
    /// İstifadəçi mövcuddur və bloklanmayıbsa <c>true</c> qaytarır.
    /// JWT köhnəlmiş ola biləcəyi üçün canlı yoxlama lazımdır.
    /// </summary>
    Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default);
}
