using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Application.Common.Settings;

namespace TapAi.Module.Identity.Infrastructure.Services;

public sealed class RedisOtpService(
    IConnectionMultiplexer redis,
    IOptions<OtpSettings> options) : IOtpService
{
    private readonly OtpSettings _settings = options.Value;

    public int MaxAttempts => _settings.MaxAttempts;

    private static string Key(string phone) => $"otp:register:{phone}";

    public async Task StoreAsync(string phone, string code, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(new OtpRecord(code, 0));
        await db.StringSetAsync(Key(phone), json, TimeSpan.FromSeconds(_settings.ExpirySeconds));
    }

    public async Task<(OtpRecord? Record, TimeSpan? Ttl)> GetAsync(string phone, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var result = await db.StringGetWithExpiryAsync(Key(phone));

        if (result.Value.IsNullOrEmpty)
            return (null, null);

        var record = JsonSerializer.Deserialize<OtpRecord>((string)result.Value!);
        return (record, result.Expiry);
    }

    public async Task UpdateAsync(string phone, OtpRecord record, TimeSpan ttl, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(record);
        await db.StringSetAsync(Key(phone), json, ttl);
    }

    public async Task DeleteAsync(string phone, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(Key(phone));
    }
}
