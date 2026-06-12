import { expect, test } from '@playwright/test';
import { ShopPage } from '../pages/shop-page';

// Port of "US-02 Warenkorb: Produkt erfolgreich in den Warenkorb legen".
test('added product appears in cart with matching total', async ({ page }) => {
  const shop = new ShopPage(page);

  await shop.openHome();
  await shop.search('Combination Pliers');
  await shop.openProduct('Combination Pliers');
  const price = await shop.unitPrice();
  await shop.addToCart();

  await shop.openCart();

  await expect(shop.cartRows).toHaveCount(1);
  expect(await shop.total()).toBeCloseTo(price, 2);
});
