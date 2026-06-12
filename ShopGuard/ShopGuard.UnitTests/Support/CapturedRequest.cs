namespace ShopGuard.UnitTests.Support;

/// <summary>
/// Snapshot of an outgoing HTTP request captured by the mocked handler.
/// The body is materialized eagerly because HttpClient disposes request content after sending.
/// </summary>
public sealed record CapturedRequest(
    HttpMethod Method,
    Uri? Uri,
    string? Body,
    IReadOnlyDictionary<string, string> Headers);
