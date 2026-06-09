using System.Reflection;
using FluentValidation;
using TapAi.Shared.Application.Abstraction;
using TapAi.Shared.Application.Pipeline;
using TapAi.Shared.Application.ResponseObject;
using TapAi.Shared.Application.ResponseObject.Concreate;
using TapAi.Shared.Application.ResponseObject.Enums;

namespace TapAi.Shared.Infrastructure.Pipeline;

public sealed class ValidationPipelineBehavior<TCommand, TResponse>(
    IEnumerable<IValidator<TCommand>> validators)
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Response
{
    public async Task<TResponse> HandleAsync(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(command, ct)));

        var errors = results
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
            return CreateValidationError(errors);

        return await next();
    }

    /// <summary>
    /// Konkret tipi kompilyasiya zamanı bilmədən validation-error TResponse yaradır.
    /// Həm <see cref="Response"/>, həm də <see cref="Response{T}"/> statik
    /// <c>ValidationError(IEnumerable&lt;CustomValidationError&gt;)</c> factory-si təqdim edir —
    /// onu MethodInfo axtarışı vasitəsilə çağırırıq.
    /// </summary>
    private static TResponse CreateValidationError(IEnumerable<CustomValidationError> errors)
    {
        var type = typeof(TResponse);

        if (type == typeof(Response))
            return (TResponse)(object)Response.ValidationError(errors);

        var method = type.GetMethod(
            nameof(Response.ValidationError),
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            [typeof(IEnumerable<CustomValidationError>)]);

        // Ehtiyat yol: əgər TResponse öz factory-sini elan etməyən hansısa alt-sinifdirsə,
        // düzgün status ilə Response yaradıb cast edirik.
        if (method is null)
            return (TResponse)(object)new Response(ResponseStatusCode.ValidationError)
            {
                ValidationErrors = errors
            };

        return (TResponse)method.Invoke(null, [errors])!;
    }
}