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

    const getLaunchReportButton = page.getByRole('button', { name: 'Launch Report' });
    await getLaunchReportButton.click();
});

test('has singleIdentityPopup', async ({ page }) => {
    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: "Direct patient care" });

    const getLaunchReportButton = page.getByRole('button', { name: 'Launch Report' });
    await getLaunchReportButton.click();

    const getSingleIdentityButton = 
        page.getByRole('cell', { name: 'Test single record click: 1111151000' });

    await getSingleIdentityButton.click();

    await page.waitForTimeout(5000); // 5-second wait
});

test('has identityColoumnWithMultipleRecords', async ({ page }) => {
    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: "Direct patient care" });

    const getLaunchReportButton = page.getByRole('button', { name: 'Launch Report' });
    await getLaunchReportButton.click();

    const getSingleIdentityButton = 
        page.getByRole('cell', { name: 'Test multiple record click: 1111192000,1111209000' });

    await getSingleIdentityButton.click();
});
