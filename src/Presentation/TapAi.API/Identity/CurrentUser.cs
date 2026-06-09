using System.Security.Claims;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Application.Exceptions;

namespace TapAi.API.Identity;

/// <summary>
/// HTTP konteksti üzərindən cari istifadəçinin kimliyini JWT claim-lərindən oxuyur.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid Id =>
        IdOrDefault ?? throw new UnauthorizedException("İstifadəçi kimliyi tapılmadı.");

    public Guid? IdOrDefault =>
        Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;

    public bool IsAdmin => Principal?.IsInRole("Admin") ?? false;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
}
