namespace TapAi.Module.Identity.Application.Common.Interfaces;

public sealed record RegistrationClaims(
    string PhoneNumber,
    string PasswordHash,
    string FirstName,
    string LastName);

public interface IRegistrationJwtService
{
    string GenerateToken(string phoneNumber, string passwordHash, string firstName, string lastName);
    RegistrationClaims? ValidateToken(string token);
}
