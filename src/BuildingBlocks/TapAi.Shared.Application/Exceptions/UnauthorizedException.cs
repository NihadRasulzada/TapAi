namespace TapAi.Shared.Application.Exceptions;

/// <summary>
/// Tələb olunan istifadəçi kimliyi token-də yoxdur və ya etibarsızdır.
/// HTTP layında 401-ə map olunur.
/// </summary>
public sealed class UnauthorizedException(string message = "Unauthorized.") : Exception(message);
