using TapAi.Module.Identity.Domain.Events;

namespace TapAi.Module.Identity.Application.Common.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : DomainEvent;
}