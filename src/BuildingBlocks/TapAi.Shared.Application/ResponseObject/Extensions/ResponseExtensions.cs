using TapAi.Shared.Application.ResponseObject.Abstraction;
using TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Shared.Application.ResponseObject.Extensions;

public static class ResponseExtensions
{
    /// <summary>
    /// Cavab uğurludursa verilmiş əməliyyatı icra edir
    /// </summary>
    public static IResponse<T> OnSuccess<T>(this IResponse<T> response, Action<T> action)
    {
        if (response.IsSuccess && response.Data != null)
        {
            action(response.Data);
        }
        return response;
    }

    /// <summary>
    /// Cavab uğursuzdursa verilmiş əməliyyatı icra edir
    /// </summary>
    public static IResponse<T> OnFailure<T>(this IResponse<T> response, Action<IResponse<T>> action)
    {
        if (response.IsFailure)
        {
            action(response);
        }
        return response;
    }

    /// <summary>
    /// Datanı bir tipdən digərinə map edir
    /// </summary>
    public static Response<TResult> Map<T, TResult>(
        this IResponse<T> response,
        Func<T, TResult> mapper
    )
    {
        if (response.IsSuccess && response.Data != null)
        {
            return Response<TResult>.Success(mapper(response.Data), response.Message);
        }

        return new Response<TResult>((Response)response, default(TResult));
    }

    /// <summary>
    /// Cavabda validation xətalarının olub-olmadığını yoxlayır
    /// </summary>
    public static bool HasValidationErrors(this IResponse response)
    {
        return response.ValidationErrors != null && response.ValidationErrors.Any();
    }

    /// <summary>
    /// Cavabda xətaların olub-olmadığını yoxlayır
    /// </summary>
    public static bool HasErrors(this IResponse response)
    {
        return response.Errors != null && response.Errors.Any();
    }

    /// <summary>
    /// Varsa, ilk validation xəta mesajını qaytarır
    /// </summary>
    public static string GetFirstValidationError(this IResponse response)
    {
        return response.ValidationErrors?.FirstOrDefault()?.ErrorMessage;
    }

    /// <summary>
    /// Varsa, ilk xətanın təsvirini qaytarır
    /// </summary>
    public static string GetFirstError(this IResponse response)
    {
        return response.Errors?.FirstOrDefault()?.Description;
    }

    /// <summary>
    /// İki cavabı birləşdirir; varsa, uğursuz olanı saxlayır
    /// </summary>
    public static Response<T> Combine<T>(this IResponse<T> first, IResponse second)
    {
        if (first.IsFailure)
            return (Response<T>)first;

        if (second.IsFailure)
            return new Response<T>((Response)second, first.Data);

        return (Response<T>)first;
    }

    /// <summary>
    /// Funksiyanı icra edib cavab qaytarır
    /// </summary>
    public static Response<TResult> Bind<T, TResult>(
        this IResponse<T> response,
        Func<T, Response<TResult>> func
    )
    {
        if (response.IsFailure)
        {
            return new Response<TResult>((Response)response, default(TResult));
        }

        return func(response.Data);
    }

    /// <summary>
    /// Açara görə metadata dəyərini qaytarır
    /// </summary>
    public static TValue GetMetadata<TValue>(this IResponse response, string key)
    {
        if (response.Metadata != null && response.Metadata.TryGetValue(key, out var value))
        {
            if (value is TValue typedValue)
                return typedValue;
        }
        return default(TValue);
    }

    /// <summary>
    /// Metadata-da açarın olub-olmadığını yoxlayır
    /// </summary>
    public static bool HasMetadata(this IResponse response, string key)
    {
        return response.Metadata != null && response.Metadata.ContainsKey(key);
    }
}