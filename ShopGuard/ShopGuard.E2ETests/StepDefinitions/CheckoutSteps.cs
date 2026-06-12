using AwesomeAssertions;
using Reqnroll;
using ShopGuard.E2ETests.Pages;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.StepDefinitions;

[Binding]
public sealed class CheckoutSteps(PlaywrightDriver driver, ScenarioState state)
{
    private CartPage CartPage => new(driver.Page);
    private CheckoutPage CheckoutPage => new(driver.Page);

    [When("ich zur Kasse gehe")]
    public async Task WhenProceedToCheckout()
    {
        await CartPage.OpenAsync();
        await CartPage.ProceedToCheckoutAsync();
        await CheckoutPage.ConfirmSignedInAsync();
    }

    [When("ich den Checkout mit Standardadresse und Zahlung per Nachnahme abschließe")]
    public async Task WhenCompleteCheckoutWithDefaults()
    {
        await CheckoutPage.CompleteBillingAddressAsync(houseNumber: "1");
        await CheckoutPage.PayWithCashOnDeliveryAsync();
    }

    [Then("erhalte ich eine Bestellbestätigung mit Rechnungsnummer")]
    public async Task ThenOrderConfirmationWithInvoiceNumber()
    {
        var invoiceNumber = await CheckoutPage.GetInvoiceNumberAsync();
        invoiceNumber.Should().MatchRegex(@"^INV-\d+$");
        state.OrderNumber = invoiceNumber;
    }
}
