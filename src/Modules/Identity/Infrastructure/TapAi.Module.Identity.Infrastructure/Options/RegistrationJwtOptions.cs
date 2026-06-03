using System.ComponentModel.DataAnnotations;

namespace TapAi.Module.Identity.Infrastructure.Options;

public sealed class RegistrationJwtOptions
{
    public const string SectionName = "RegistrationJwt";

    [Required] public string SecretKey { get; init; } = string.Empty;
    [Range(60, 3600)] public int ExpirySeconds { get; init; } = 600;
}
