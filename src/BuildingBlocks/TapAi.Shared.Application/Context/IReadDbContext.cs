namespace TapAi.Shared.Application.Context;

/// <summary>
/// Yalnız oxuma (query) verilənlər bazası kontekstləri üçün marker interfeysi.
/// İmplementasiyalar <c>SaveChanges</c> və ya <c>SaveChangesAsync</c> təqdim etməməlidir.
/// </summary>
public interface IReadDbContext { }