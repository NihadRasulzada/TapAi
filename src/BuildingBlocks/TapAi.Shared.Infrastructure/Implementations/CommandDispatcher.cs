using Microsoft.Extensions.DependencyInjection;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Application.Pipeline;
using TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Shared.Infrastructure.Implementations;

public sealed class CommandDispatcher(IServiceProvider sp) : ICommandDispatcher
{
    public Task<TResponse> DispatchAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>
        where TResponse : Response
    {
        var handler = sp.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        var behaviors = sp.GetServices<IPipelineBehavior<TCommand, TResponse>>().ToList();

        CommandHandlerDelegate<TResponse> pipeline = () => handler.HandleAsync(command, ct);

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = pipeline;
            var current = behavior;
            pipeline = () => current.HandleAsync(command, next, ct);
        }

        return pipeline();
    }
}