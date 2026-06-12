using Microsoft.Playwright;
using ShopGuard.Core.Helpers;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Page object for step 1 (cart) of the Toolshop checkout wizard (/checkout).
/// </summary>
public sealed class CartPage(IPage page) : BasePage(page)
{
    private ILocator CartRows => Page.Locator("[data-test='product-title']");
    private ILocator CartTotal => Page.Locator("[data-test='cart-total']");
    private ILocator RemoveButtons => Page.Locator("table a.btn-danger");
    private ILocator EmptyCartMessage => Page.GetByText("The cart is empty");
    private ILocator ProceedFromCartButton => Page.Locator("[data-test='proceed-1']");

    public async Task OpenAsync()
    {
        await NavigateAsync("checkout");
        // Wait until either cart lines or the empty message rendered.
        await CartRows.First.Or(EmptyCartMessage.First).WaitForAsync();
    }

    public Task<int> GetRowCountAsync() => CartRows.CountAsync();

    /// <summary>Reads the displayed cart total as decimal.</summary>
    public async Task<decimal> GetTotalAsync()
        => PriceCalculator.ParseDisplayedPrice(await CartTotal.InnerTextAsync());

    /// <summary>Removes the first cart line and waits for the cart to re-render.</summary>
    public async Task RemoveFirstItemAsync()
    {
        var rowsBefore = await GetRowCountAsync();
        await RemoveButtons.First.ClickAsync();
        // Either fewer rows or the empty-cart message must appear.
        if (rowsBefore == 1)
        {
            await EmptyCartMessage.First.WaitForAsync();
        }
    }

    public Task<bool> IsEmptyAsync() => EmptyCartMessage.First.IsVisibleAsync();

    /// <summary>Proceeds from the cart step to the sign-in step of the wizard.</summary>
    public Task ProceedToCheckoutAsync() => ProceedFromCartButton.ClickAsync();
}
