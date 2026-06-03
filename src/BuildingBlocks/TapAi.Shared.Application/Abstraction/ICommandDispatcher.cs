using TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Shared.Application.Abstraction;

public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>
        where TResponse : Response;
}