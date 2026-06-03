namespace TapAi.Module.Identity.Application.Common.Settings;

public sealed class SeedAdminSettings
{
    public const string SectionName = "AdminSeed";

    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
