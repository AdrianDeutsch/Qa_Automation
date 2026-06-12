using Reqnroll;
using ShopGuard.E2ETests.Database;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.Hooks;

/// <summary>
/// Cleans up order rows created by @db scenarios so every run starts isolated.
/// </summary>
[Binding]
public sealed class DatabaseHooks(DatabaseContext database, ScenarioState state)
{
    [AfterScenario("@db")]
    public void RemoveScenarioOrders()
    {
        if (state.CustomerEmail is not null)
        {
            database.Orders.DeleteByCustomer(state.CustomerEmail);
        }
    }
}
