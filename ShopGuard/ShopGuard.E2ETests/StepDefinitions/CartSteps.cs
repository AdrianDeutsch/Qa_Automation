using AwesomeAssertions;
using Reqnroll;
using ShopGuard.Core.Helpers;
using ShopGuard.E2ETests.Pages;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.StepDefinitions;

[Binding]
public sealed class CartSteps(PlaywrightDriver driver, ScenarioState state)
{
    private SearchResultsPage SearchResultsPage => new(driver.Page);
    private ProductPage ProductPage => new(driver.Page);
    private CartPage CartPage => new(driver.Page);

    [Given("ich das Produkt {string} über die Suche in den Warenkorb gelegt habe")]
    [When("ich das Produkt {string} über die Suche in den Warenkorb lege")]
    public async Task WhenAddProductToCartViaSearch(string productTitle)
    {
        // Toolshop products have no stable URL slugs, so every product is opened via search.
        await driver.Page.GotoAsync(TestConfiguration.Settings.ShopBaseUrl);
        await SearchResultsPage.SearchAsync(productTitle);
        await SearchResultsPage.OpenProductAsync(productTitle);
        await AddCurrentProductToCartAsync();
    }

    [When("ich das Produkt in den Warenkorb lege")]
    public Task WhenAddCurrentProductToCart() => AddCurrentProductToCartAsync();

    [When("ich das Produkt aus dem Warenkorb entferne")]
    public async Task WhenRemoveProductFromCart()
    {
        await CartPage.OpenAsync();
        await CartPage.RemoveFirstItemAsync();
    }

    [Then("zeigt der Warenkorb {int} Artikel an")]
    public async Task ThenCartShowsItemCount(int expectedCount)
    {
        await CartPage.OpenAsync();
        (await CartPage.GetRowCountAsync()).Should().Be(expectedCount);
    }

    [Then("die Gesamtsumme entspricht der Summe der hinzugefügten Produkte")]
    public async Task ThenCartTotalMatchesAddedProducts()
    {
        var expected = PriceCalculator.CalculateCartTotal(
            state.AddedProductPrices.Select((price, index) => new CartItem($"Item {index}", price, 1)));

        (await CartPage.GetTotalAsync()).Should().Be(expected);
    }

    [Then("ist der Warenkorb leer")]
    public async Task ThenCartIsEmpty()
        => (await CartPage.IsEmptyAsync()).Should().BeTrue();

    private async Task AddCurrentProductToCartAsync()
    {
        state.AddedProductPrices.Add(await ProductPage.GetPriceAsync());
        await ProductPage.AddToCartAsync();
    }
}
