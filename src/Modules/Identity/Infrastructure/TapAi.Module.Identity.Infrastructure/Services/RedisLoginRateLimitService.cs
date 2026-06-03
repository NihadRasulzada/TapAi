using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Application.Common.Settings;

namespace TapAi.Module.Identity.Infrastructure.Services;

public sealed class RedisLoginRateLimitService(
    IConnectionMultiplexer redis,
    IOptions<BruteForceSettings> options) : ILoginRateLimitService
{
    private readonly BruteForceSettings _settings = options.Value;

    private static string Key(string phone) => $"login:rl:{phone}";

    public async Task<bool> IsBlockedAsync(string phone, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(Key(phone));
        return value.TryParse(out long count) && count >= _settings.MaxFailedAttempts;
    }

    public async Task RecordFailedAttemptAsync(string phone, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var key = Key(phone);
        var count = await db.StringIncrementAsync(key);

        // İlk uğursuz cəhddə TTL tənzimlə; sonrakılar mövcud pəncərəni davam etdirir
        if (count == 1)
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(_settings.BlockDurationSeconds));
    }

    public async Task ResetAsync(string phone, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(Key(phone));
    }
}
