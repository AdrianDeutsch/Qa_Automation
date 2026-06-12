using System.Text.Json.Serialization;

namespace ShopGuard.Core.Models;

/// <summary>
/// Booking payload as defined by the restful-booker API contract.
/// Property names are mapped explicitly because the API uses all-lowercase JSON keys.
/// </summary>
public sealed class Booking
{
    [JsonPropertyName("firstname")]
    public required string FirstName { get; init; }

    [JsonPropertyName("lastname")]
    public required string LastName { get; init; }

    [JsonPropertyName("totalprice")]
    public required int TotalPrice { get; init; }

    [JsonPropertyName("depositpaid")]
    public required bool DepositPaid { get; init; }

    [JsonPropertyName("bookingdates")]
    public required BookingDates BookingDates { get; init; }

    [JsonPropertyName("additionalneeds")]
    public string? AdditionalNeeds { get; init; }
}
