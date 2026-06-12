using System.Net;

namespace ShopGuard.Core.Api;

/// <summary>
/// Thrown when the API returns an unexpected status code.
/// Carries the status code and raw response body for diagnostics in test reports.
/// </summary>
public sealed class ApiException(HttpStatusCode statusCode, string? responseBody, string message)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string? ResponseBody { get; } = responseBody;
}
