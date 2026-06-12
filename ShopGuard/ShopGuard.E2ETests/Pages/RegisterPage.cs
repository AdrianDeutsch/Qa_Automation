using Microsoft.Playwright;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Page object for /auth/register. Used to create throwaway accounts so login and
/// checkout scenarios never depend on pre-existing shared test users.
/// </summary>
public sealed class RegisterPage(IPage page) : BasePage(page)
{
    public Task OpenAsync() => NavigateAsync("auth/register");

    /// <summary>
    /// Registers a new customer account. The shop redirects to the login page on success.
    /// </summary>
    public async Task RegisterAsync(string firstName, string lastName, string email, string password)
    {
        await Page.Locator("[data-test='first-name']").FillAsync(firstName);
        await Page.Locator("[data-test='last-name']").FillAsync(lastName);
        await Page.Locator("[data-test='dob']").FillAsync("1990-01-15");
        await Page.Locator("[data-test='street']").FillAsync("Teststraße");
        await Page.Locator("[data-test='house_number']").FillAsync("1");
        await Page.Locator("[data-test='postal_code']").FillAsync("10115");
        await Page.Locator("[data-test='city']").FillAsync("Berlin");
        await Page.Locator("[data-test='state']").FillAsync("Berlin");
        await Page.Locator("[data-test='country']").SelectOptionAsync(new SelectOptionValue { Label = "Germany" });
        await Page.Locator("[data-test='phone']").FillAsync("03012345678");
        await Page.Locator("[data-test='email']").FillAsync(email);
        await Page.Locator("[data-test='password']").FillAsync(password);
        await Page.Locator("[data-test='register-submit']").ClickAsync();
        await Page.WaitForURLAsync("**/auth/login");
    }
}
