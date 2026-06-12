using AwesomeAssertions;
using ShopGuard.Core.Helpers;

namespace ShopGuard.UnitTests.Helpers;

[TestFixture]
public sealed class TestDataGeneratorTests
{
    [Test]
    public void UniqueEmail_GeneratesDistinctAddresses()
    {
        var emails = Enumerable.Range(0, 50).Select(_ => TestDataGenerator.UniqueEmail()).ToList();

        emails.Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void UniqueEmail_UsesReservedExampleDomain()
    {
        TestDataGenerator.UniqueEmail("checkout").Should()
            .StartWith("checkout.").And.EndWith("@example.com");
    }

    [Test]
    public void DefaultBooking_HasCheckOutAfterCheckIn()
    {
        var booking = TestDataGenerator.DefaultBooking();

        booking.BookingDates.CheckOut.Should().BeAfter(booking.BookingDates.CheckIn);
        booking.BookingDates.CheckIn.Should().BeAfter(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Test]
    public void DefaultBooking_UsesProvidedFirstName()
    {
        TestDataGenerator.DefaultBooking("Maria").FirstName.Should().Be("Maria");
    }

    [Test]
    public void DefaultBooking_HasPositivePrice()
    {
        TestDataGenerator.DefaultBooking().TotalPrice.Should().BePositive();
    }
}
