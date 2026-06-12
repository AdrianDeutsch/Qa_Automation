using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Single element of the GET /booking listing response.
/// </summary>
public sealed class BookingId
{
    [JsonPropertyName("bookingid")]
    public required int Id { get; init; }
}
