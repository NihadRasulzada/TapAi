using TapAi.Shared.Application.Context;

namespace TapAi.Module.Identity.Persistence.Contexts;

/// <summary>
/// Identity verilənlər bazası üçün yazma konteksti.
/// Yalnız <see cref="IWriteDbContext"/>-dən gələn ümumi mutasiya köməkçilərini təqdim edir.
/// Command handler-lər dəyişdirmədən əvvəl entity oxumaq lazım olduqda
/// <see cref="IIdentityReadDbContext"/>-i inject etməlidir.
/// </summary>
public interface IIdentityWriteDbContext : IWriteDbContext { }
