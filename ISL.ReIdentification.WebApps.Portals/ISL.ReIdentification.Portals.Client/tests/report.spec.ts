import { test, expect } from '@playwright/test';

test.beforeEach(async ({ page }) => {
    await page.goto('https://localhost:5173/report/fake/fake/fake');
});

test('has title', async ({ page }) => {
    await expect(page).toHaveTitle("Re-Identification Portal");
});

test('has reasonForReIdentify', async ({ page }) => {
    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: "Direct patient care" });

    const getNhsNumberButton = page.getByRole('button', { name: 'Launch Report' });
    await getNhsNumberButton.click();
});

test('has singleIdentityPopup', async ({ page }) => {
    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: "Direct patient care" });

    const getNhsNumberButton = page.getByRole('button', { name: 'Launch Report' });
    await getNhsNumberButton.click();
});