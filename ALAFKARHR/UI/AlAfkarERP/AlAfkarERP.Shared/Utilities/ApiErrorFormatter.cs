using AlAfkarERP.Shared.Dtos;
using System.Net;
using System.Text.Json;

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
        error.Title = string.IsNullOrWhiteSpace(error.Title) ? fallback.Title : error.Title;
    }

    private static string? GetSafeBackendDetail(ErrorResponseDto error)
    {
        if (string.IsNullOrWhiteSpace(error.Detail))
        {
            return null;
        }

        var detail = error.Detail.Trim();

        if (detail.StartsWith("{") || detail.StartsWith("[") || detail.Contains("System.", StringComparison.OrdinalIgnoreCase))
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
            .Where(message => !string.IsNullOrWhiteSpace(message))
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
}
