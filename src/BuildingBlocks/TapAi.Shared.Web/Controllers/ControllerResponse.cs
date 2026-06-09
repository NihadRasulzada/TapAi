using TapAi.Shared.Application.ResponseObject;

namespace TapAi.Shared.Web.Controllers;

/// <summary>
/// Data daşımayan standart uğur cavabı
/// </summary>
public class SuccessResponse(string message)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Əməliyyatın nəticəsini təsvir edən uğur mesajı
    /// </summary>
    /// <example>Entity uğurla yaradıldı</example>
    public string Message { get; set; } = message;
}

public class SuccessResponse<T>(T data, string message)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    public T Data { get; set; } = data;

    /// <summary>
    /// Əməliyyatın nəticəsini təsvir edən uğur mesajı
    /// </summary>
    /// <example>Entity uğurla yaradıldı</example>
    public string Message { get; set; } = message;
}

public class CreatedResponse<T>(T data, string message) : SuccessResponse<T>(data, message) { }

/// <summary>
/// Klient xətaları üçün xəta cavabı (400, 404)
/// </summary>
public class ErrorResponse(string message)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Nəyin səhv getdiyini təsvir edən xəta mesajı
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6 ID-li entity tapılmadı</example>
    public string Message { get; set; } = message;
}

/// <summary>
/// Validation xətası cavabı (422)
/// </summary>
public class ValidationErrorResponse(string message, Dictionary<string, string[]> errors)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Xəta mesajı
    /// </summary>
    /// <example>Validation uğursuz oldu</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// Property adına görə qruplaşdırılmış validation xətaları
    /// </summary>
    /// <example>{"name":["Ad tələb olunur","Ad unikal olmalıdır"],"description":["Təsvir 500 simvoldan çox olmamalıdır"]}</example>
    public Dictionary<string, string[]> Errors { get; set; } = errors;
}

/// <summary>
/// Daxili server xətası cavabı (500)
/// </summary>
public class ServerErrorResponse(string message, IEnumerable<CustomError> errors)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Xəta mesajı
    /// </summary>
    /// <example>Daxili server xətası baş verdi</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// Əlavə xəta detalları (opsional)
    /// </summary>
    public IEnumerable<CustomError>? Errors { get; set; } = errors;
}

/// <summary>
/// Data yükü ilə standart uğur cavabı
/// </summary>
/// <typeparam name="T">Qaytarılan datanın tipi</typeparam>
public class PagedDataResponse<T>(T? data, string message, PaginationMetadata metadata)
{
    /// <summary>
    /// Əməliyyatın uğurlu olub-olmadığını göstərir
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Əməliyyatın nəticəsini təsvir edən uğur mesajı
    /// </summary>
    /// <example>Əməliyyat uğurla tamamlandı</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// Qaytarılan data yükü
    /// </summary>
    public T? Data { get; set; } = data;

    public PaginationMetadata PaginationMetadata { get; set; } = metadata;
}

public class NotFoundResponse(string message)
{
    public string Message { get; set; } = message;
}

public class ConflictResponse(string message)
{
    public string Message { get; set; } = message;
}

public class BadRequestResponse(string message)
{
    public string Message { get; set; } = message;
}

public class PaginationMetadata(
    int pageIndex,
    int pageSize,
    int totalCount,
    int totalPages,
    bool hasPreviousPage,
    bool hasNextPage
)
{
    public int PageIndex { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int TotalPages { get; set; } = totalPages;
    public bool HasPreviousPage { get; set; } = hasPreviousPage;
    public bool HasNextPage { get; set; } = hasNextPage;
}