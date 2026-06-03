namespace TapAi.Module.Identity.Application.Common.Settings;

public sealed class BruteForceSettings
{
    public const string SectionName = "BruteForce";

    public int MaxFailedAttempts { get; init; } = 5;
    public int BlockDurationSeconds { get; init; } = 900;
}
