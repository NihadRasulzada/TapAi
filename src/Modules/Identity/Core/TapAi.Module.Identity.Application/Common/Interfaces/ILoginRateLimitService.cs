namespace TapAi.Module.Identity.Application.Common.Interfaces;

public interface ILoginRateLimitService
{
    Task<bool> IsBlockedAsync(string phone, CancellationToken ct = default);
    Task RecordFailedAttemptAsync(string phone, CancellationToken ct = default);
    Task ResetAsync(string phone, CancellationToken ct = default);
}
