namespace ShopGuard.Core.Helpers;

/// <summary>
/// Price arithmetic used to verify cart and checkout totals shown in the UI.
/// All amounts are decimal and rounded to two places (MidpointRounding.ToEven,
/// matching the shop's banker's rounding).
/// </summary>
public static class PriceCalculator
{
    /// <summary>Calculates the total of a single cart line.</summary>
    public static decimal CalculateLineTotal(decimal unitPrice, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        return Math.Round(unitPrice * quantity, 2, MidpointRounding.ToEven);
    }

    /// <summary>Calculates the cart subtotal across all lines.</summary>
    public static decimal CalculateCartTotal(IEnumerable<CartItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return items.Sum(item => CalculateLineTotal(item.UnitPrice, item.Quantity));
    }

    /// <summary>Applies a percentage discount (0–100) to a total.</summary>
    public static decimal ApplyDiscount(decimal total, decimal discountPercent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        if (discountPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(discountPercent), discountPercent, "Discount must be between 0 and 100 percent.");
        }

        return Math.Round(total * (1 - discountPercent / 100m), 2, MidpointRounding.ToEven);
    }

    /// <summary>
    /// Parses a price string as rendered by the shop UI (e.g. "$1,200.00" or "1.200,00 €")
    /// into a decimal value.
    /// </summary>
    public static decimal ParseDisplayedPrice(string displayedPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayedPrice);

        // Strip currency symbols and whitespace, keep digits and separators.
        var cleaned = new string(displayedPrice.Where(c => char.IsDigit(c) || c is '.' or ',').ToArray());
        if (cleaned.Length == 0)
        {
            throw new FormatException($"'{displayedPrice}' contains no numeric value.");
        }

        // Decide which separator is the decimal one: the right-most of '.' and ','.
        var lastDot = cleaned.LastIndexOf('.');
        var lastComma = cleaned.LastIndexOf(',');
        var decimalSeparator = lastDot > lastComma ? '.' : ',';
        var groupSeparator = decimalSeparator == '.' ? ',' : '.';

        cleaned = cleaned.Replace(groupSeparator.ToString(), string.Empty);
        cleaned = cleaned.Replace(decimalSeparator, '.');

        return decimal.Parse(cleaned, System.Globalization.CultureInfo.InvariantCulture);
    }
}
