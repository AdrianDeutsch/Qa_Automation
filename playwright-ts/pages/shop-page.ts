import { Locator, Page } from '@playwright/test';

/**
 * Combined page object for catalog search, product detail and cart —
 * the TypeScript port keeps it intentionally compact (three scenarios only).
 */
export class ShopPage {
  readonly productTitles: Locator;
  readonly noResultsMessage: Locator;
  readonly cartTotal: Locator;
  readonly cartRows: Locator;

  constructor(private readonly page: Page) {
    this.productTitles = page.locator("a.card [data-test='product-name']");
    this.noResultsMessage = page.getByText('There are no products found');
    this.cartTotal = page.locator("[data-test='cart-total']");
    this.cartRows = page.locator("[data-test='product-title']");
  }

  async openHome(): Promise<void> {
    await this.page.goto('/');
  }

  async search(term: string): Promise<void> {
    await this.page.locator("[data-test='search-query']").fill(term);
    await this.page.locator("[data-test='search-submit']").click();
    // Same two-stage wait as the C# BasePage: caption first, then an outcome.
    await this.page.getByText('Searched for:').waitFor();
    await this.page
      .locator('a.card')
      .first()
      .or(this.noResultsMessage.first())
      .first()
      .waitFor();
  }

  async openProduct(title: string): Promise<void> {
    await this.page.locator('a.card', { hasText: title }).first().click();
    await this.page.locator("[data-test='add-to-cart']").waitFor();
  }

  async unitPrice(): Promise<number> {
    const text = await this.page.locator("[data-test='unit-price']").innerText();
    return Number.parseFloat(text.replace(/[^\d.]/g, ''));
  }

  async addToCart(): Promise<void> {
    await this.page.locator("[data-test='add-to-cart']").click();
    await this.page.getByText('Product added to shopping cart').first().waitFor();
  }

  async openCart(): Promise<void> {
    await this.page.goto('/checkout');
    await this.cartRows.first().waitFor();
  }

  async total(): Promise<number> {
    const text = await this.cartTotal.innerText();
    return Number.parseFloat(text.replace(/[^\d.]/g, ''));
  }
}
