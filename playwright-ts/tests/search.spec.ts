import { expect, test } from '@playwright/test';
import { ShopPage } from '../pages/shop-page';

// Port of "Produktsuche: Suche findet vorhandene Produkte" and
// "Suche ohne Treffer zeigt einen Hinweis".
test.describe('product search', () => {
  test('finds existing products', async ({ page }) => {
    const shop = new ShopPage(page);

    await shop.openHome();
    await shop.search('Pliers');

    const titles = await shop.productTitles.allInnerTexts();
    expect(titles.length).toBeGreaterThanOrEqual(1);
    expect(titles.some((t) => t.toLowerCase().includes('pliers'))).toBe(true);
  });

  test('shows a hint when nothing matches', async ({ page }) => {
    const shop = new ShopPage(page);

    await shop.openHome();
    await shop.search('xyzgibtesnicht123');

    await expect(shop.noResultsMessage.first()).toBeVisible();
  });
});
