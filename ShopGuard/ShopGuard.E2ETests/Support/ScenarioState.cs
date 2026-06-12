using ShopGuard.Core.Models;

namespace ShopGuard.E2ETests.Support;

/// <summary>
/// Scenario-scoped state bag shared between step definition classes
/// (injected by Reqnroll context injection instead of stringly-typed ScenarioContext keys).
/// </summary>
public sealed class ScenarioState
{
    // UI state
    public string? RegisteredEmail { get; set; }
    public string? RegisteredPassword { get; set; }
    public decimal ExpectedCartTotal { get; set; }
    public readonly List<decimal> AddedProductPrices = [];

    // API state
    public Booking? RequestBooking { get; set; }
    public BookingCreatedResponse? CreatedBooking { get; set; }
    public Booking? FetchedBooking { get; set; }
    public string? AuthToken { get; set; }
    public int LastStatusCode { get; set; }

    // Database state
    public string? OrderNumber { get; set; }
    public string? CustomerEmail { get; set; }
}
