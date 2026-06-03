namespace TapAi.Module.Identity.Application.Common.Interfaces;

public sealed record OtpRecord(string Code, int Attempts);

public interface IOtpService
{
    int MaxAttempts { get; }
    Task StoreAsync(string phone, string code, CancellationToken ct = default);
    Task<(OtpRecord? Record, TimeSpan? Ttl)> GetAsync(string phone, CancellationToken ct = default);
    Task UpdateAsync(string phone, OtpRecord record, TimeSpan ttl, CancellationToken ct = default);
    Task DeleteAsync(string phone, CancellationToken ct = default);
}
