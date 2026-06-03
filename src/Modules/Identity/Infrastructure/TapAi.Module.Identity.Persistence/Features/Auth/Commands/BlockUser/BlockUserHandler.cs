using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;
using AppConc = TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed class BlockUserHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb
) : ICommandHandler<BlockUserRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        BlockUserRequest command, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
            return AppConc.Response.NotFound($"User '{command.UserId}' not found.");

        writeDb.Attach(user);
        user.Block(command.DurationSeconds);
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response.Success("User blocked successfully.");
    }
}
