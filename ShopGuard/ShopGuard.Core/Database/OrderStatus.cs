namespace ShopGuard.Core.Database;

/// <summary>
/// Allowed order states. Stored as strings to keep the SQL readable in test assertions.
/// </summary>
public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Shipped = "Shipped";
    public const string Cancelled = "Cancelled";
}
