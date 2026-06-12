using Microsoft.Playwright;

namespace ShopGuard.E2ETests.Pages;

/// <summary>
/// Page object for /auth/login of the Toolshop demo.
/// </summary>
public sealed class LoginPage(IPage page) : BasePage(page)
{
    private ILocator EmailInput => Page.Locator("[data-test='email']");
    private ILocator PasswordInput => Page.Locator("[data-test='password']");
    private ILocator LoginButton => Page.Locator("[data-test='login-submit']");
    private ILocator LoginError => Page.Locator("[data-test='login-error']");
    private ILocator EmailFieldError => Page.Locator("[data-test='email-error']");

    public Task OpenAsync() => NavigateAsync("auth/login");

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    /// <summary>Returns the summary error shown for rejected credentials.</summary>
    public async Task<string> GetErrorMessageAsync()
    {
        await LoginError.WaitForAsync();
        return await LoginError.InnerTextAsync();
    }

    /// <summary>Returns the inline validation error below the e-mail field, if any.</summary>
    public async Task<string> GetEmailValidationErrorAsync()
        => await EmailFieldError.IsVisibleAsync()
            ? await EmailFieldError.InnerTextAsync()
            : string.Empty;
}
