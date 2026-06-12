import { Locator, Page } from '@playwright/test';

/**
 * Page object for /auth/login of the Toolshop demo.
 * Mirrors the C# LoginPage so both worlds stay structurally identical.
 */
export class LoginPage {
  readonly loginError: Locator;
  readonly userNavMenu: Locator;

  constructor(private readonly page: Page) {
    this.loginError = page.locator("[data-test='login-error']");
    this.userNavMenu = page.locator("[data-test='nav-menu']");
  }

  async open(): Promise<void> {
    await this.page.goto('/auth/login');
  }

  async login(email: string, password: string): Promise<void> {
    await this.page.locator("[data-test='email']").fill(email);
    await this.page.locator("[data-test='password']").fill(password);
    await this.page.locator("[data-test='login-submit']").click();
  }
}
