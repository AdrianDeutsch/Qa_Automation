using AwesomeAssertions;
using ShopGuard.Core.Database;

namespace ShopGuard.UnitTests.Database;

[TestFixture]
public sealed class OrderRepositoryTests
{
    private string _dbPath = null!;
    private OrderRepository _repository = null!;

    [SetUp]
    public void CreateDatabase()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"shopguard-test-{Guid.NewGuid():N}.db");
        _repository = new OrderRepository($"Data Source={_dbPath}");
        _repository.EnsureSchema();
    }

    [TearDown]
    public void DeleteDatabase()
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        File.Delete(_dbPath);
    }

    [Test]
    public void InsertOrder_CreatesRowWithPendingStatus()
    {
        _repository.InsertOrder("ORD-1001", "anna@example.com", 49.90m);

        var order = _repository.GetByOrderNumber("ORD-1001");

        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Pending);
        order.Total.Should().Be(49.90m);
        order.CustomerEmail.Should().Be("anna@example.com");
    }

    [Test]
    public void GetByOrderNumber_UnknownOrder_ReturnsNull()
    {
        _repository.GetByOrderNumber("ORD-MISSING").Should().BeNull();
    }

    [Test]
    public void UpdateStatus_ChangesExistingOrder()
    {
        _repository.InsertOrder("ORD-1002", "ben@example.com", 10m);

        _repository.UpdateStatus("ORD-1002", OrderStatus.Shipped);

        _repository.GetByOrderNumber("ORD-1002")!.Status.Should().Be(OrderStatus.Shipped);
    }

    [Test]
    public void UpdateStatus_UnknownOrder_Throws()
    {
        var act = () => _repository.UpdateStatus("ORD-MISSING", OrderStatus.Paid);

        act.Should().Throw<InvalidOperationException>().WithMessage("*ORD-MISSING*");
    }

    [Test]
    public void CountByCustomer_CountsOnlyMatchingRows()
    {
        _repository.InsertOrder("ORD-1", "carla@example.com", 1m);
        _repository.InsertOrder("ORD-2", "carla@example.com", 2m);
        _repository.InsertOrder("ORD-3", "dave@example.com", 3m);

        _repository.CountByCustomer("carla@example.com").Should().Be(2);
    }

    [Test]
    public void DeleteByCustomer_RemovesAllOrdersOfCustomer()
    {
        _repository.InsertOrder("ORD-4", "erin@example.com", 1m);
        _repository.InsertOrder("ORD-5", "erin@example.com", 2m);

        _repository.DeleteByCustomer("erin@example.com");

        _repository.CountByCustomer("erin@example.com").Should().Be(0);
    }

    [Test]
    public void InsertOrder_DuplicateOrderNumber_Throws()
    {
        _repository.InsertOrder("ORD-6", "fred@example.com", 1m);

        var act = () => _repository.InsertOrder("ORD-6", "fred@example.com", 1m);

        act.Should().Throw<Microsoft.Data.Sqlite.SqliteException>();
    }
}
