namespace TapAi.Module.Identity.Application.Common.Settings;

public sealed class OtpSettings
{
    public const string SectionName = "Otp";

    public int ExpirySeconds { get; init; } = 300;
    public string DefaultCode { get; init; } = string.Empty;
    public int MaxAttempts { get; init; } = 3;
}
