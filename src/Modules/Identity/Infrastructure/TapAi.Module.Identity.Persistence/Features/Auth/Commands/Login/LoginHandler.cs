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
    IOptions<BruteForceSettings> bruteForceOptions
) : ICommandHandler<LoginRequest, AppConc.Response<LoginResponse>>
{
    public async Task<AppConc.Response<LoginResponse>> HandleAsync(
        LoginRequest command, CancellationToken ct = default)
    {
        var normalized = command.PhoneNumber.ToUpperInvariant();

        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedPhoneNumber == normalized, ct);

        if (user is null)
            return AppConc.Response<LoginResponse>.Unauthorized("Phone number or password is incorrect.");

        if (user.IsCurrentlyBlocked())
            return AppConc.Response<LoginResponse>.Forbidden("User is blocked. Please contact support.");

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            writeDb.Attach(user);
            user.RecordFailedLogin();
            if (user.FailedLoginCount >= bruteForceOptions.Value.MaxFailedAttempts)
                user.Block(bruteForceOptions.Value.BlockDurationSeconds);
            await writeDb.SaveChangesAsync(ct);

            return AppConc.Response<LoginResponse>.Unauthorized("Phone number or password is incorrect.");
        }

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
