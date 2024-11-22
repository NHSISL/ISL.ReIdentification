import { test, expect, type Page } from '@playwright/test';

test.beforeEach(async ({ page }) => {
    await page.goto('https://localhost:5173/home');
});

test('has title', async ({ page }) => {
    // Expect a title "to contain" a substring.
    await expect(page).toHaveTitle("Re-Identification Portal");
});


