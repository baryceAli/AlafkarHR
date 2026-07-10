using AlAfkarERP.Shared.Dtos;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AlAfkarERP.Shared.Utilities;

public static class ApiErrorFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ErrorResponseDto FromHttpError(HttpStatusCode statusCode, string content)
    {
        var status = (int)statusCode;
        var error = TryDeserializeProblemDetails(content) ?? new ErrorResponseDto
        {
            Status = status,
            Title = "Request failed"
        };

        error.Status = error.Status == 0 ? status : error.Status;

        ApplyFriendlyMessages(error);

        return error;
    }

    public static ErrorResponseDto FromClientException(Exception exception)
    {
        var status = exception switch
        {
            TaskCanceledException => 408,
            HttpRequestException => 503,
            _ => 500
        };

        var error = new ErrorResponseDto
        {
            Status = status,
            Title = "Client Error"
        };

        ApplyFriendlyMessages(error);

        return error;
    }

    public static ErrorResponseDto FromResponseParseException(Exception exception)
    {
        return new ErrorResponseDto
        {
            Status = 502,
            Title = "Response Error",
            Detail = "The server response could not be read. Please refresh and try again.",
            UserMessageEn = "The server response could not be read. Please refresh and try again.",
            UserMessageAr = "تعذرت قراءة استجابة الخادم. يرجى تحديث الصفحة والمحاولة مرة أخرى."
        };
    }

    public static ErrorResponseDto FromClientException(Exception exception, int status)
    {
        var error = new ErrorResponseDto
        {
            Status = status,
            Title = "Client Error",
            Detail = exception.Message
        };

        ApplyFriendlyMessages(error);

        return error;
    }

    public static string GetDisplayMessage(ErrorResponseDto? error, string language, string fallback)
    {
        var message = SanitizePublicMessage(error?.GetDisplayMessage(language));

        return string.IsNullOrWhiteSpace(message) || HasInternalDetails(message)
            ? fallback
            : message;
    }

    private static ErrorResponseDto? TryDeserializeProblemDetails(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<ErrorResponseDto>(content, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static void ApplyFriendlyMessages(ErrorResponseDto error)
    {
        var backendDetail = GetSafeBackendDetail(error);
        var fallback = GetFallback(error.Status);
        var validationMessage = FormatValidationErrors(error.Errors);

        error.UserMessageEn = FirstNotEmpty(validationMessage, backendDetail, fallback.En);
        error.UserMessageAr = FirstNotEmpty(validationMessage, backendDetail, fallback.Ar);
        error.Detail = error.UserMessageEn;
        error.Title = FirstNotEmpty(SanitizePublicMessage(error.Title), fallback.Title);
        error.Instance = string.Empty;
        error.TraceId = string.Empty;
    }

    private static string? GetSafeBackendDetail(ErrorResponseDto error)
    {
        if (string.IsNullOrWhiteSpace(error.Detail))
        {
            return null;
        }

        var detail = SanitizePublicMessage(error.Detail);

        if (string.IsNullOrWhiteSpace(detail) ||
            detail.StartsWith("{") ||
            detail.StartsWith("[") ||
            HasInternalDetails(detail))
        {
            return null;
        }

        return detail;
    }

    private static string? FormatValidationErrors(Dictionary<string, string[]>? errors)
    {
        if (errors is null || errors.Count == 0)
        {
            return null;
        }

        var messages = errors
            .SelectMany(error => error.Value)
            .Select(SanitizePublicMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message) && !HasInternalDetails(message))
            .Distinct()
            .Take(5)
            .ToList();

        return messages.Count == 0 ? null : string.Join(" ", messages);
    }

    private static (string Title, string En, string Ar) GetFallback(int status) => status switch
    {
        400 => ("Bad Request", "Please check the entered data and try again.", "يرجى مراجعة البيانات المدخلة والمحاولة مرة أخرى."),
        401 => ("Unauthorized", "Your session has expired. Please sign in again.", "انتهت صلاحية الجلسة. يرجى تسجيل الدخول مرة أخرى."),
        403 => ("Forbidden", "You do not have permission to perform this action.", "ليست لديك صلاحية لتنفيذ هذا الإجراء."),
        404 => ("Not Found", "The requested record was not found.", "لم يتم العثور على السجل المطلوب."),
        408 => ("Request Timeout", "The request took too long. Please try again.", "استغرق الطلب وقتا طويلا. يرجى المحاولة مرة أخرى."),
        500 => ("Internal Server Error", "An unexpected error occurred. Please try again or contact support.", "حدث خطأ غير متوقع. يرجى المحاولة مرة أخرى أو التواصل مع الدعم."),
        503 => ("Service Unavailable", "Unable to connect to the server. Please check your connection and try again.", "تعذر الاتصال بالخادم. يرجى التحقق من الاتصال والمحاولة مرة أخرى."),
        _ => ("Request Failed", "The request could not be completed. Please try again.", "تعذر إكمال الطلب. يرجى المحاولة مرة أخرى.")
    };

    private static string FirstNotEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? "";

    public static bool HasInternalDetails(string message)
        => message.Contains("System.", StringComparison.OrdinalIgnoreCase) ||
           message.Contains("AlAfkarERP.", StringComparison.OrdinalIgnoreCase) ||
           message.Contains("SharedWithUI.", StringComparison.OrdinalIgnoreCase) ||
           message.Contains("Microsoft.", StringComparison.OrdinalIgnoreCase) ||
           message.Contains(" at ", StringComparison.OrdinalIgnoreCase) ||
           message.Contains("\\", StringComparison.Ordinal);

    public static string? SanitizePublicMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var sanitized = message.Trim();
        sanitized = Regex.Replace(sanitized, @"\((?:https?://)?(?:localhost|127\.0\.0\.1|::1)[^)]+\)", "", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"https?://[^\s,)]+", "", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\b(?:localhost|127\.0\.0\.1|::1):\d+\b", "", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\b(?:[A-Za-z_]\w*\.){2,}[A-Za-z_]\w*\b", "");
        sanitized = Regex.Replace(sanitized, @"[A-Za-z]:\\[^\s,;]+", "");
        sanitized = Regex.Replace(sanitized, @"\+?\s*statusCode\s*:\s*[A-Za-z0-9]+", "", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\s+", " ").Trim(' ', ',', ';', '-', '+');

        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }
}
