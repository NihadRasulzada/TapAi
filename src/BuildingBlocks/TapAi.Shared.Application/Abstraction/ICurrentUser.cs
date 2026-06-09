namespace TapAi.Shared.Application.Abstraction;

/// <summary>
/// Autentifikasiya olunmuş istifadəçinin token-dən oxunan kimliyi.
/// Composition root-da HTTP konteksti üzərindən implement olunur.
/// </summary>
public interface ICurrentUser
{
    /// <summary>İstifadəçi Id-si. Token yoxdursa/xətalıdırsa <c>UnauthorizedException</c> atır.</summary>
    Guid Id { get; }

    /// <summary>İstifadəçi Id-si, yoxdursa <c>null</c> (atmır).</summary>
    Guid? IdOrDefault { get; }

    bool IsAdmin { get; }

    bool IsAuthenticated { get; }
}