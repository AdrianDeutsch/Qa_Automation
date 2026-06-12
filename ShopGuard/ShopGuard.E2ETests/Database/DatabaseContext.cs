using ShopGuard.Core.Database;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.Database;

/// <summary>
/// Scenario-scoped access to the order validation database.
/// The schema is created on first access so scenarios stay order-independent.
/// </summary>
public sealed class DatabaseContext
{
    private readonly Lazy<OrderRepository> _repository = new(() =>
    {
        var repository = new OrderRepository(TestConfiguration.Settings.DbConnectionString);
        repository.EnsureSchema();
        return repository;
    });

    public OrderRepository Orders => _repository.Value;
}
