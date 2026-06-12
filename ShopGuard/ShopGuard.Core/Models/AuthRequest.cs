using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Credentials payload for POST /auth.
/// </summary>
public sealed class AuthRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }
}
