using Dapper;
using Microsoft.Data.Sqlite;

namespace ShopGuard.Core.Database;

/// <summary>
/// Dapper-based access to the order validation database (SQLite locally and in CI;
/// the SQL is kept ANSI-compatible so it also runs against SQL Server).
/// </summary>
public sealed class OrderRepository(string connectionString)
{
    /// <summary>Creates the Orders table when it does not exist yet.</summary>
    public void EnsureSchema()
    {
        using var connection = Open();
        connection.Execute(
            """
            CREATE TABLE IF NOT EXISTS Orders (
                Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderNumber   TEXT NOT NULL UNIQUE,
                CustomerEmail TEXT NOT NULL,
                Total         NUMERIC NOT NULL,
                Status        TEXT NOT NULL,
                CreatedAtUtc  TEXT NOT NULL
            )
            """);
    }

    /// <summary>Inserts a new order with status <see cref="OrderStatus.Pending"/>.</summary>
    public void InsertOrder(string orderNumber, string customerEmail, decimal total)
    {
        using var connection = Open();
        connection.Execute(
            """
            INSERT INTO Orders (OrderNumber, CustomerEmail, Total, Status, CreatedAtUtc)
            VALUES (@orderNumber, @customerEmail, @total, @status, @createdAtUtc)
            """,
            new
            {
                orderNumber,
                customerEmail,
                total,
                status = OrderStatus.Pending,
                createdAtUtc = DateTime.UtcNow.ToString("O"),
            });
    }

    /// <summary>Updates the status of an existing order.</summary>
    public void UpdateStatus(string orderNumber, string newStatus)
    {
        using var connection = Open();
        var affected = connection.Execute(
            "UPDATE Orders SET Status = @newStatus WHERE OrderNumber = @orderNumber",
            new { newStatus, orderNumber });

        if (affected == 0)
        {
            throw new InvalidOperationException($"Order '{orderNumber}' not found.");
        }
    }

    /// <summary>Reads a single order by its business key, or null when absent.</summary>
    public Order? GetByOrderNumber(string orderNumber)
    {
        using var connection = Open();
        return connection.QuerySingleOrDefault<Order>(
            "SELECT Id, OrderNumber, CustomerEmail, Total, Status, CreatedAtUtc FROM Orders WHERE OrderNumber = @orderNumber",
            new { orderNumber });
    }

    /// <summary>Counts orders for a customer; used for test data isolation checks.</summary>
    public int CountByCustomer(string customerEmail)
    {
        using var connection = Open();
        return connection.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Orders WHERE CustomerEmail = @customerEmail",
            new { customerEmail });
    }

    /// <summary>Removes all orders of a customer; used to clean up scenario data.</summary>
    public void DeleteByCustomer(string customerEmail)
    {
        using var connection = Open();
        connection.Execute(
            "DELETE FROM Orders WHERE CustomerEmail = @customerEmail",
            new { customerEmail });
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
