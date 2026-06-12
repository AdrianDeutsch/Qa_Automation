using Microsoft.Playwright;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Page object for the catalog search results of the Toolshop demo.
/// </summary>
public sealed class SearchResultsPage(IPage page) : BasePage(page)
{
    private ILocator ProductCards => Page.Locator("a.card");
    private ILocator ProductTitles => Page.Locator("a.card [data-test='product-name']");
    private ILocator NoResultMessage => Page.GetByText("There are no products found");

    public async Task<IReadOnlyList<string>> GetProductTitlesAsync()
        => (await ProductTitles.AllInnerTextsAsync()).Select(t => t.Trim()).ToList();

    public async Task<bool> HasNoResultsAsync()
    {
        try
        {
            // The message renders asynchronously after the grid update.
            await NoResultMessage.First.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>Opens a product detail page by its displayed title.</summary>
    public async Task OpenProductAsync(string productTitle)
    {
        await ProductCards.Filter(new LocatorFilterOptions { HasTextString = productTitle }).First.ClickAsync();
        await Page.Locator("[data-test='add-to-cart']").WaitForAsync();
    }
}
