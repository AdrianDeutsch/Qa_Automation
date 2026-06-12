using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Response of POST /auth. The API returns either a token (success)
/// or a reason string ("Bad credentials") with HTTP 200 — a known API quirk.
/// </summary>
public sealed class AuthResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
