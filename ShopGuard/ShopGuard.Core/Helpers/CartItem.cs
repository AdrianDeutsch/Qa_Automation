namespace ShopGuard.Core.Helpers;

/// <summary>
/// Lightweight cart line used by <see cref="PriceCalculator"/> and UI assertions.
/// </summary>
public sealed record CartItem(string ProductName, decimal UnitPrice, int Quantity);
