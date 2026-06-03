namespace TapAi.Module.Identity.Domain.Events;

public sealed class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public UserRegisteredEvent(Guid userId, string firstName, string lastName)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }
}
