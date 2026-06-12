import { expect, test } from '@playwright/test';
import { LoginPage } from '../pages/login-page';

// Port of "US-01 Login: Login mit falschem Passwort wird abgelehnt".
// Uses a non-existing account on purpose: the shop answers with the same
// generic error for unknown users and wrong passwords (no enumeration leak).
test('login with invalid credentials shows generic error', async ({ page }) => {
  const loginPage = new LoginPage(page);

  await loginPage.open();
  await loginPage.login('ghost.user.shopguard@example.com', 'Falsch!2026x');

  await expect(loginPage.loginError).toContainText('Invalid email or password');
  await expect(loginPage.userNavMenu).toBeHidden();
});
