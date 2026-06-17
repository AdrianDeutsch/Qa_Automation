using ShopGuard.Core.Models;

namespace ShopGuard.Core.Helpers;

/// <summary>
/// Deterministic-enough test data factory. Every value carries a unique suffix
/// so parallel test runs never collide on user accounts or bookings.
/// </summary>
public static class TestDataGenerator
{
    // Process-wide monotonic counter so suffixes are unique even when many are
    // requested within the same millisecond (a plain timestamp + random number
    // can collide under the birthday paradox).
    private static int _sequence;

    /// <summary>Creates a unique e-mail address on a test-reserved domain.</summary>
    public static string UniqueEmail(string prefix = "shopguard")
        => $"{prefix}.{UniqueSuffix()}@example.com";

    /// <summary>Creates a guaranteed-unique alphanumeric identifier (timestamp + atomic counter).</summary>
    public static string UniqueSuffix()
        => $"{DateTime.UtcNow:yyyyMMddHHmmssfff}{Interlocked.Increment(ref _sequence)}";

    /// <summary>Creates a valid default booking with a stay in the future.</summary>
    public static Booking DefaultBooking(string? firstName = null)
    {
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        return new Booking
        {
            FirstName = firstName ?? $"Guard{Random.Shared.Next(100, 999)}",
            LastName = "ShopTester",
            TotalPrice = Random.Shared.Next(50, 500),
            DepositPaid = true,
            BookingDates = new BookingDates { CheckIn = checkIn, CheckOut = checkIn.AddDays(3) },
            AdditionalNeeds = "Breakfast",
        };
    }
}
