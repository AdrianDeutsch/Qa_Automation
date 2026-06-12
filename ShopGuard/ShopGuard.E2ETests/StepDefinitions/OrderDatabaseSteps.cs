using AwesomeAssertions;
using Reqnroll;
using ShopGuard.Core.Database;
using ShopGuard.Core.Helpers;
using ShopGuard.E2ETests.Database;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.StepDefinitions;

[Binding]
public sealed class OrderDatabaseSteps(DatabaseContext database, ScenarioState state)
{
    [Given("ein Kunde mit einer eindeutigen E-Mail-Adresse")]
    public void GivenCustomerWithUniqueEmail()
        => state.CustomerEmail = TestDataGenerator.UniqueEmail("order");

    [When("das Backend die Bestellung zur Buchung registriert")]
    public void WhenBackendRegistersOrderForBooking()
    {
        // Mirrors what the shop backend does after a successful POST /booking:
        // persist an order row referencing the booking id.
        state.OrderNumber = $"BK-{state.CreatedBooking!.BookingId}-{TestDataGenerator.UniqueSuffix()}";
        database.Orders.InsertOrder(
            state.OrderNumber, state.CustomerEmail!, state.CreatedBooking.Booking.TotalPrice);
    }

    [Given("ich für den Kunden eine Bestellung über {decimal} angelegt habe")]
    [When("ich für den Kunden eine Bestellung über {decimal} anlege")]
    public void WhenCreateOrderForCustomer(decimal total)
    {
        state.OrderNumber = $"ORD-{TestDataGenerator.UniqueSuffix()}";
        database.Orders.InsertOrder(state.OrderNumber, state.CustomerEmail!, total);
    }

    [When("ich die Bestellung storniere")]
    public void WhenCancelOrder()
        => database.Orders.UpdateStatus(state.OrderNumber!, OrderStatus.Cancelled);

    [Then("existiert in der Tabelle Orders genau ein Datensatz für den Kunden")]
    public void ThenExactlyOneOrderRowExists()
        => database.Orders.CountByCustomer(state.CustomerEmail!).Should().Be(1);

    [Then("hat die Bestellung den Status {string}")]
    public void ThenOrderHasStatus(string expectedStatus)
    {
        var order = database.Orders.GetByOrderNumber(state.OrderNumber!);
        order.Should().NotBeNull();
        order.Status.Should().Be(expectedStatus);
    }
}
