using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Check-in/check-out date pair of a booking. Serialized as "yyyy-MM-dd" by the API.
/// </summary>
public sealed class BookingDates
{
    [JsonPropertyName("checkin")]
    public required DateOnly CheckIn { get; init; }

    [JsonPropertyName("checkout")]
    public required DateOnly CheckOut { get; init; }
}
