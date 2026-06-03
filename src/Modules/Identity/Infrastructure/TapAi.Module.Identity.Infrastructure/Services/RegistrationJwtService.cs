using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TapAi.Module.Identity.Application.Common.Interfaces;
using TapAi.Module.Identity.Infrastructure.Options;

namespace TapAi.Module.Identity.Infrastructure.Services;

public sealed class RegistrationJwtService(IOptions<RegistrationJwtOptions> options) : IRegistrationJwtService
{
    private const string TokenType = "registration";
    private const string ClaimType = "type";
    private const string ClaimPhone = "phone";
    private const string ClaimPasswordHash = "ph";

    private readonly RegistrationJwtOptions _opts = options.Value;

    public string GenerateToken(string phoneNumber, string passwordHash, string firstName, string lastName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimType, TokenType),
            new Claim(ClaimPhone, phoneNumber),
            new Claim(ClaimPasswordHash, passwordHash),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_opts.ExpirySeconds),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RegistrationClaims? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.SecretKey));

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParams, out _);

            if (principal.FindFirstValue(ClaimType) != TokenType)
                return null;

            var phone = principal.FindFirstValue(ClaimPhone);
            var passwordHash = principal.FindFirstValue(ClaimPasswordHash);
            var firstName = principal.FindFirstValue(JwtRegisteredClaimNames.GivenName)
                         ?? principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = principal.FindFirstValue(JwtRegisteredClaimNames.FamilyName)
                        ?? principal.FindFirstValue(ClaimTypes.Surname);

            if (phone is null || passwordHash is null || firstName is null || lastName is null)
                return null;

            return new RegistrationClaims(phone, passwordHash, firstName, lastName);
        }
        catch
        {
            return null;
        }
    }
}
