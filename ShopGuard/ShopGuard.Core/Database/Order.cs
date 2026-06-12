namespace ShopGuard.Core.Database;

/// <summary>
/// Persisted order row in the Orders table of the validation database.
/// </summary>
public sealed class Order
{
    public long Id { get; init; }

    public required string OrderNumber { get; init; }

    public required string CustomerEmail { get; init; }

    public required decimal Total { get; init; }

    public required string Status { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
