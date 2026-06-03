using TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Shared.Application.Abstraction;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Response
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}