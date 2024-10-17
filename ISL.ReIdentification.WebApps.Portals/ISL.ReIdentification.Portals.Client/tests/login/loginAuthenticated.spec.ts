import { test, expect } from '@playwright/test';

test('has title', async ({ page }) => {
  await page.goto('https://playwright.dev/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle(/Playwright/);
});

test('get started link', async ({ page }) => {
  await page.goto('https://playwright.dev/');

  // Click the get started link.
  await page.getByRole('link', { name: 'Get started' }).click();

  // Expects page to have a heading with the name of Installation.
  await expect(page.getByRole('heading', { name: 'Installation' })).toBeVisible();
});

test('check title and login', async ({ browser }) => {
    // Launch a new context with ignoreHTTPSErrors set to true
    const context = await browser.newContext({ ignoreHTTPSErrors: true });
    const page = await context.newPage();

    // Call the network interceptor function
    //await createNetworkInterceptor(page);

    await page.goto('https://localhost:5173/home');

    // Click the "Sign In" button which triggers the AAD login popup
    const [popup] = await Promise.all([
        context.waitForEvent('page'), // Wait for the popup to appear
        page.click('button:has-text("Sign In")'), // Click the "Sign In" button
    ]);

    // Wait for the popup to load the AAD login page
    await popup.waitForLoadState();

    // Perform login actions in the popup
    //await popup.fill('input[name="loginfmt"]', '');
    //await popup.click('input[type="submit"]');
    //await popup.waitForLoadState();
    //await popup.fill('input[name="passwd"]', '');
    //await popup.click('input[type="submit"]');
});

