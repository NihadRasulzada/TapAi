using TapAi.Shared.Domain.Models;

namespace TapAi.Module.Identity.Domain.Entity;

public class User : BaseEntity
{
    public string? PhoneNumber { get; private set; }
    public string? NormalizedPhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsAdmin { get; private set; }
    public bool IsBlocked { get; private set; }
    public int FailedLoginCount { get; private set; }
    public long? BlockedUntilSeconds { get; private set; }

    private User(Guid id) : base(id) { }

    public static User CreateWithPhone(
        string phoneNumber,
        string passwordHash,
        string firstName,
        string lastName)
    {
        return new User(Guid.NewGuid())
        {
            PhoneNumber = phoneNumber,
            NormalizedPhoneNumber = phoneNumber.ToUpperInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsAdmin = false,
            IsBlocked = false,
            FailedLoginCount = 0
        };
    }

    public static User CreateAdmin(
        string phoneNumber,
        string passwordHash,
        string firstName,
        string lastName)
    {
        return new User(Guid.NewGuid())
        {
            PhoneNumber = phoneNumber,
            NormalizedPhoneNumber = phoneNumber.ToUpperInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsAdmin = true,
            IsBlocked = false,
            FailedLoginCount = 0
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Block(int durationSeconds)
    {
        IsBlocked = true;
        BlockedUntilSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + durationSeconds;
    }

    public void Unblock()
    {
        IsBlocked = false;
        BlockedUntilSeconds = null;
        FailedLoginCount = 0;
    }

    public void RecordFailedLogin()
    {
        FailedLoginCount++;
    }

    public void OnSuccessfulLogin()
    {
        FailedLoginCount = 0;

        if (IsBlocked && !IsCurrentlyBlocked())
        {
            IsBlocked = false;
            BlockedUntilSeconds = null;
        }
    }

    public bool IsCurrentlyBlocked()
    {
        if (!IsBlocked) return false;
        if (BlockedUntilSeconds is null) return true;
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < BlockedUntilSeconds;
    }
}
