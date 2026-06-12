using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Response envelope returned by POST /booking.
/// </summary>
public sealed class BookingCreatedResponse
{
    [JsonPropertyName("bookingid")]
    public required int BookingId { get; init; }

    [JsonPropertyName("booking")]
    public required Booking Booking { get; init; }
}
