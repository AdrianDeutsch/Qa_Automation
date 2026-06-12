using Microsoft.Playwright;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Common base for all page objects of the Toolshop demo
/// (https://practicesoftwaretesting.com). The shop exposes stable
/// data-test attributes which all locators rely on.
/// </summary>
public abstract class BasePage(IPage page)
{
    protected IPage Page { get; } = page;

    protected static string BaseUrl => TestConfiguration.Settings.ShopBaseUrl.TrimEnd('/');

    private ILocator CartQtyBadge => Page.Locator("[data-test='cart-quantity']");
    private ILocator SearchBox => Page.Locator("[data-test='search-query']");
    private ILocator SearchButton => Page.Locator("[data-test='search-submit']");
    private ILocator UserNavMenu => Page.Locator("[data-test='nav-menu']");

    protected async Task NavigateAsync(string relativePath)
    {
        await Page.GotoAsync($"{BaseUrl}/{relativePath.TrimStart('/')}");
    }

    /// <summary>Reads the item count from the header cart badge.</summary>
    public async Task<int> GetCartItemCountAsync()
    {
        await CartQtyBadge.WaitForAsync();
        return int.Parse((await CartQtyBadge.InnerTextAsync()).Trim());
    }

    /// <summary>Runs a search via the home page search box.</summary>
    public async Task SearchAsync(string searchTerm)
    {
        await SearchBox.FillAsync(searchTerm);
        await SearchButton.ClickAsync();
        // Two-stage wait: the caption proves the search was applied (otherwise stale
        // home-page cards would match), then the grid shows either results or the
        // explicit no-results message.
        await Page.GetByText("Searched for:").WaitForAsync();
        await Page.Locator("a.card").First
            .Or(Page.GetByText("There are no products found").First)
            .First.WaitForAsync();
    }

    /// <summary>True when the header shows the customer menu, i.e. a customer is signed in.</summary>
    public Task<bool> IsLoggedInAsync() => UserNavMenu.IsVisibleAsync();

    /// <summary>Waits until the customer menu rendered (the SPA updates the header asynchronously).</summary>
    public Task WaitForLoggedInAsync() => UserNavMenu.WaitForAsync();
}
