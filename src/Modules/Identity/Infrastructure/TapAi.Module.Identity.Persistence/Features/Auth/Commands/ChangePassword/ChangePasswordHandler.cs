using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher
) : ICommandHandler<ChangePasswordRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        ChangePasswordRequest command, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
            return AppConc.Response.NotFound($"User '{command.UserId}' not found.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            return AppConc.Response.Unauthorized("Current password is incorrect.");

        // Şifrəni yenilə
        writeDb.Attach(user);
        user.ChangePassword(passwordHasher.Hash(command.NewPassword));

        // Şifrə dəyişdikdə mövcud bütün refresh token-ları revoke et;
        // oğurlanmış sessiya davam etməsin.
        var activeTokens = await readDb.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == command.UserId && !rt.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            writeDb.Attach(token);
            token.Revoke();
        }

        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response.Success("Password changed successfully.");
    }
}
