using AwesomeAssertions;
using Reqnroll;
using ShopGuard.Core.Helpers;
using ShopGuard.E2ETests.Pages;
using ShopGuard.E2ETests.Support;

namespace ShopGuard.E2ETests.StepDefinitions;

[Binding]
public sealed class LoginSteps(PlaywrightDriver driver, ScenarioState state)
{
    private LoginPage LoginPage => new(driver.Page);
    private RegisterPage RegisterPage => new(driver.Page);

    private const string DefaultPassword = "ShopGuard!2026x";

    [Given("ich besitze ein registriertes Kundenkonto")]
    public async Task GivenRegisteredCustomerAccount()
    {
        state.RegisteredEmail = TestDataGenerator.UniqueEmail("login");
        state.RegisteredPassword = DefaultPassword;

        await RegisterPage.OpenAsync();
        await RegisterPage.RegisterAsync("Shop", "Guard", state.RegisteredEmail, state.RegisteredPassword);
    }

    [Given("ich bin auf der Login-Seite")]
    public Task GivenOnLoginPage() => LoginPage.OpenAsync();

    [Given("ich bin als Kunde angemeldet")]
    public async Task GivenLoggedInCustomer()
    {
        await LoginPage.OpenAsync();
        await LoginPage.LoginAsync(state.RegisteredEmail!, state.RegisteredPassword!);
        await driver.Page.WaitForURLAsync("**/account");
    }

    [When("ich mich mit gültigen Zugangsdaten anmelde")]
    public async Task WhenLoginWithValidCredentials()
    {
        await LoginPage.LoginAsync(state.RegisteredEmail!, state.RegisteredPassword!);
        await driver.Page.WaitForURLAsync("**/account");
    }

    [When("ich mich mit dem falschen Passwort {string} anmelde")]
    public Task WhenLoginWithWrongPassword(string wrongPassword)
        => LoginPage.LoginAsync(state.RegisteredEmail!, wrongPassword);

    [When("ich mich mit der E-Mail {string} und dem Passwort {string} anmelde")]
    public Task WhenLoginWithCredentials(string email, string password)
        => LoginPage.LoginAsync(email, password);

    [Then("bin ich als Kunde angemeldet")]
    public async Task ThenLoggedIn()
    {
        await LoginPage.WaitForLoggedInAsync();
        (await LoginPage.IsLoggedInAsync()).Should().BeTrue();
    }

    [Then("bin ich nicht angemeldet")]
    public async Task ThenNotLoggedIn()
        => (await LoginPage.IsLoggedInAsync()).Should().BeFalse();

    [Then("sehe ich die Fehlermeldung {string}")]
    public async Task ThenErrorMessageShown(string expectedFragment)
        => (await LoginPage.GetErrorMessageAsync()).Should().Contain(expectedFragment);

    [Then("sehe ich einen Validierungsfehler am E-Mail-Feld")]
    public async Task ThenEmailValidationErrorShown()
        => (await LoginPage.GetEmailValidationErrorAsync()).Should().NotBeEmpty();
}
