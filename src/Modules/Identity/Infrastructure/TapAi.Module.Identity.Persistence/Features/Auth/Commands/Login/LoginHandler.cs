using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Application.Common.Settings;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;
using RefreshTokenEntity = TapAi.Module.Identity.Domain.Entity.RefreshToken;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed class LoginHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    ILoginRateLimitService rateLimitService,
    IOptions<BruteForceSettings> bruteForceOptions
) : ICommandHandler<LoginRequest, AppConc.Response<LoginResponse>>
{
    public async Task<AppConc.Response<LoginResponse>> HandleAsync(
        LoginRequest command, CancellationToken ct = default)
    {
        var normalized = command.PhoneNumber.ToUpperInvariant();

        // Redis rate-limit yoxlaması — mövcud olmayan nömrələrə qarşı da işləyir
        if (await rateLimitService.IsBlockedAsync(normalized, ct))
            return AppConc.Response<LoginResponse>.Forbidden(
                "Too many failed login attempts. Please try again later.");

        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedPhoneNumber == normalized, ct);

        if (user is null)
        {
            await rateLimitService.RecordFailedAttemptAsync(normalized, ct);
            return AppConc.Response<LoginResponse>.Unauthorized("Phone number or password is incorrect.");
        }

        if (user.IsCurrentlyBlocked())
            return AppConc.Response<LoginResponse>.Forbidden("User is blocked. Please contact support.");

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            // Redis sayğacı artır (mövcud olmayan nömrə halını da əhatə edir)
            await rateLimitService.RecordFailedAttemptAsync(normalized, ct);

            // DB-də per-account izləmə (admin block üçün)
            writeDb.Attach(user);
            user.RecordFailedLogin();
            if (user.FailedLoginCount >= bruteForceOptions.Value.MaxFailedAttempts)
                user.Block(bruteForceOptions.Value.BlockDurationSeconds);
            await writeDb.SaveChangesAsync(ct);

            return AppConc.Response<LoginResponse>.Unauthorized("Phone number or password is incorrect.");
        }

        // Uğurlu giriş — Redis sayğacını sıfırla, DB-ni yenilə
        await rateLimitService.ResetAsync(normalized, ct);

        writeDb.Attach(user);
        user.OnSuccessfulLogin();

        var accessToken = jwtService.GenerateAccessToken(user);
        var accessTokenExpiresAt = jwtService.GetAccessTokenExpiresAt();

        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var hashedToken     = jwtService.HashRefreshToken(rawRefreshToken);
        var refreshToken    = new RefreshTokenEntity(user.Id, hashedToken);
        var refreshTokenExpiresAt =
            DateTimeOffset.FromUnixTimeSeconds(refreshToken.ExpiresAtSeconds).UtcDateTime;

        writeDb.Add(refreshToken);
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<LoginResponse>.Success(
            new LoginResponse(accessToken, accessTokenExpiresAt, rawRefreshToken, refreshTokenExpiresAt, user.Id));
    }
}
