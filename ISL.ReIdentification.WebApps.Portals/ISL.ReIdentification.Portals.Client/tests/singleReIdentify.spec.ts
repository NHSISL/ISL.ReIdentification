import { test, expect, Page } from '@playwright/test';
import testData from './testData/testData.json' assert { type: 'json' };

async function navigateToReIdentifySinglePatient(page: Page) {
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();
    await link.click();
    const header = page.getByText('Reidentify Single Patient').nth(1);
    await expect(header).toHaveText('Reidentify Single Patient');
}

async function fillReIdentifyForm(page: Page, pseudoNumber: string, reason: string) {
    const pseudoNHSNumberInput = page.getByPlaceholder('Pseudo Number');
    await pseudoNHSNumberInput.fill(pseudoNumber);

    const reasonDropdown = page.getByRole('combobox');
    await reasonDropdown.selectOption({ label: reason });

    const getNhsNumberButton = page.getByRole('button', { name: 'Get Nhs Number' });
    await getNhsNumberButton.click();
}

test.beforeEach(async ({ page }) => {
    await page.goto('https://localhost:5173/home');
});

test('has Access to Patient', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '1111112000', 'Direct patient care');

    const startOverButton = page.getByRole('button', { name: 'Start Over' });
    //await expect(startOverButton).toBeVisible();

    const nhsNumberText = page.getByText('NHS Number: DEC1111112');
    await expect(nhsNumberText).toBeVisible();

    const hidingInText = page.getByText('Hiding in:');
    await expect(hidingInText).toBeVisible();

    await startOverButton.click();
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();

    await page.waitForTimeout(2000);

    // Check Breadcrumb
    // Log Out
});

test('has No Access to Patient', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '1111112008', 'Direct patient care');

    const startOverButton = page.getByRole('button', { name: 'Start Over' });
    await expect(startOverButton).toBeVisible();

    const noAccessText = page.getByText('It appears you tried to re-identify a patient you don\'t have access to.Please');
    await expect(noAccessText).toBeVisible();

    await startOverButton.click();
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();

    await page.waitForTimeout(2000);

    // Check Breadcrumb
    // Log Out
});

test('verifies No Access to Non-NHS Number', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '111111XXXX', 'Direct patient care');

    const startOverButton = page.getByRole('button', { name: 'Start Over' });
    await expect(startOverButton).toBeVisible();

    const noAccessText = page.getByText('It appears you tried to re-identify a patient you don\'t have access to.Please');
    await expect(noAccessText).toBeVisible();

    await startOverButton.click();
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();

    await page.waitForTimeout(2000);

    // Check Breadcrumb
    // Log Out
});

test('verifies No Access to Non-NHS Length Number', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '11111', 'Direct patient care');

    //No popup to say why??
    // fill out all and make sure button isnt active
});


test('has Access to Patient using Hex Number', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '04-93A-324', 'Direct patient care');

    const startOverButton = page.getByRole('button', { name: 'Start Over' });
    //await expect(startOverButton).toBeVisible();

    const nhsNumberText = page.getByText('NHS Number: DEC1111112');
    await expect(nhsNumberText).toBeVisible();

    const hidingInText = page.getByText('Hiding in:');
    await expect(hidingInText).toBeVisible();

    await startOverButton.click();
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();

    await page.waitForTimeout(2000);

    // Check Breadcrumb
    // Log Out
});

test('verifies No Access to Non-NHS Hex Number Number', async ({ page }) => {
    await navigateToReIdentifySinglePatient(page);
    await fillReIdentifyForm(page, '04-93A-321', 'Direct patient care');

    const startOverButton = page.getByRole('button', { name: 'Start Over' });
    await expect(startOverButton).toBeVisible();

    const noAccessText = page.getByText('It appears you tried to re-identify a patient you don\'t have access to.Please');
    await expect(noAccessText).toBeVisible();

    await startOverButton.click();
    const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
    await expect(link).toBeVisible();

    await page.waitForTimeout(2000);

    // Check Breadcrumb
    // Log Out
});



//testData.forEach(({ validPseudoNumber, reason, expectedText }) => {
//    test(`Re-identify with pseudoNumber: ${validPseudoNumber}`, async ({ page }) => {
//        const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
//        await expect(link).toBeVisible();
//        await link.click();

//        const pseudoNHSNumberInput = page.getByPlaceholder('Pseudo Number');
//        await pseudoNHSNumberInput.fill(validPseudoNumber);

//        const reasonDropdown = page.getByRole('combobox');
//        await reasonDropdown.selectOption({ label: reason });

//        const getNhsNumberButton = page.getByRole('button', { name: 'Get Nhs Number' });
//        await getNhsNumberButton.click();

//        // Assert the expected output
//        const outputText = page.getByText(expectedText);

//    });
//});

testData.forEach(({ validPseudoNumber, reason, expectedText}) => {
    test(`Has Access -  Re-identify with pseudoNumber: ${validPseudoNumber}`, async ({ page }) => {
        const link = page.getByRole('link', { name: 'Re-identify Single Patient' });
        await expect(link).toBeVisible();
        await link.click();

        const pseudoNHSNumberInput = page.getByPlaceholder('Pseudo Number');
        await pseudoNHSNumberInput.fill(validPseudoNumber);

        const reasonDropdown = page.getByRole('combobox');
        await reasonDropdown.selectOption({ label: reason });

        const getNhsNumberButton = page.getByRole('button', { name: 'Get Nhs Number' });
        await getNhsNumberButton.click();

        // Assert the expected output
        const outputText = page.getByText(expectedText);

    });
});