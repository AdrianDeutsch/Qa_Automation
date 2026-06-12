using Microsoft.Playwright;
using ShopGuard.Core.Helpers;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Page object for a product detail page of the Toolshop demo.
/// Products have no stable URL slugs, so navigation happens via search
/// (see <see cref="SearchResultsPage.OpenProductAsync"/>).
/// </summary>
public sealed class ProductPage(IPage page) : BasePage(page)
{
    private ILocator ProductName => Page.Locator("h1[data-test='product-name']");
    private ILocator PriceValue => Page.Locator("[data-test='unit-price']");
    private ILocator AddToCartButton => Page.Locator("[data-test='add-to-cart']");
    private ILocator AddedToast => Page.GetByText("Product added to shopping cart");

    public async Task<string> GetProductNameAsync()
        => (await ProductName.InnerTextAsync()).Trim();

    /// <summary>Reads the displayed unit price as decimal.</summary>
    public async Task<decimal> GetPriceAsync()
        => PriceCalculator.ParseDisplayedPrice(await PriceValue.InnerTextAsync());

    /// <summary>Adds the product to the cart and waits for the confirmation toast.</summary>
    public async Task AddToCartAsync()
    {
        await AddToCartButton.ClickAsync();
        await AddedToast.First.WaitForAsync();
    }
}
