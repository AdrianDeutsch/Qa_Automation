using AwesomeAssertions;
using ShopGuard.Core.Helpers;

namespace ShopGuard.UnitTests.Helpers;

[TestFixture]
public sealed class PriceCalculatorTests
{
    [TestCase(10.00, 3, 30.00)]
    [TestCase(0.10, 3, 0.30)]
    [TestCase(1200.00, 1, 1200.00)]
    public void CalculateLineTotal_MultipliesPriceAndQuantity(decimal price, int quantity, decimal expected)
    {
        PriceCalculator.CalculateLineTotal(price, quantity).Should().Be(expected);
    }

    [Test]
    public void CalculateLineTotal_NegativePrice_Throws()
    {
        var act = () => PriceCalculator.CalculateLineTotal(-1m, 1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCase(0)]
    [TestCase(-2)]
    public void CalculateLineTotal_NonPositiveQuantity_Throws(int quantity)
    {
        var act = () => PriceCalculator.CalculateLineTotal(10m, quantity);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void CalculateCartTotal_SumsAllLines()
    {
        var items = new[]
        {
            new CartItem("Build your own computer", 1200.00m, 1),
            new CartItem("USB cable", 9.99m, 3),
        };

        PriceCalculator.CalculateCartTotal(items).Should().Be(1229.97m);
    }

    [Test]
    public void CalculateCartTotal_EmptyCart_IsZero()
    {
        PriceCalculator.CalculateCartTotal([]).Should().Be(0m);
    }

    [TestCase(100.00, 10, 90.00)]
    [TestCase(99.99, 0, 99.99)]
    [TestCase(50.00, 100, 0.00)]
    public void ApplyDiscount_ReducesTotalByPercentage(decimal total, decimal percent, decimal expected)
    {
        PriceCalculator.ApplyDiscount(total, percent).Should().Be(expected);
    }

    [TestCase(-5)]
    [TestCase(101)]
    public void ApplyDiscount_PercentOutOfRange_Throws(decimal percent)
    {
        var act = () => PriceCalculator.ApplyDiscount(100m, percent);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestCase("$1,200.00", 1200.00)]
    [TestCase("1.200,00 €", 1200.00)]
    [TestCase("9.99", 9.99)]
    [TestCase("$0.10", 0.10)]
    public void ParseDisplayedPrice_HandlesUsAndEuFormats(string displayed, decimal expected)
    {
        PriceCalculator.ParseDisplayedPrice(displayed).Should().Be(expected);
    }

    [Test]
    public void ParseDisplayedPrice_WithoutDigits_Throws()
    {
        var act = () => PriceCalculator.ParseDisplayedPrice("free!");

        act.Should().Throw<FormatException>();
    }
}
