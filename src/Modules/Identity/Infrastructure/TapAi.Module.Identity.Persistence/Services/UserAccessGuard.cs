using Microsoft.EntityFrameworkCore;
using TapAi.Module.Identity.Persistence.Contexts;
using TapAi.Shared.Application.Abstraction;

namespace TapAi.Module.Identity.Persistence.Services;

/// <summary>
/// <see cref="IUserAccessGuard"/> implementasiyası — Identity read DB üzərindən
/// istifadəçinin mövcudluğunu və blok statusunu canlı yoxlayır.
/// </summary>
public sealed class UserAccessGuard(IIdentityReadDbContext readDb) : IUserAccessGuard
{
    public async Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        // Müvəqqəti blok müddəti bitibsə IsCurrentlyBlocked false qaytarır.
        return user is not null && !user.IsCurrentlyBlocked();
    }
}
