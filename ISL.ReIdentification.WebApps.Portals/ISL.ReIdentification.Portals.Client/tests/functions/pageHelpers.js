//// pageHelpers.js
//import path from 'path';
//import { expect } from '@playwright/test';

//const __filename = fileURLToPath(import.meta.url);
//const __dirname = path.dirname(__filename);

//export async function navigateToReIdentifySinglePatient(page) {
//    const link = page.getByRole('link', { name: 'Re-identify Dataset' });
//    await expect(link).toBeVisible();
//    await link.click();
//    const header = page.getByText('Dataset Upload');
//    await expect(header).toHaveText('Dataset Upload');
//}

//export async function fillReIdentifyForm(page, recipientEmail, reason) {
//    const emailAddressInput = page.getByPlaceholder('Enter user email address');
//    await emailAddressInput.fill(recipientEmail);

//    const selectEmailButton = page.getByRole('button', { name: 'Select' });
//    await expect(selectEmailButton).toBeVisible();
//    await expect(selectEmailButton).toBeEnabled();
//    await selectEmailButton.click();

//    const hasHeaderCheckbox = page.getByLabel('Has Header Record');
//    await expect(hasHeaderCheckbox).toBeVisible();
//    if (!(await hasHeaderCheckbox.isChecked())) {
//        await hasHeaderCheckbox.check();
//    }

//    const fileInput = page.locator('#csvUpload');
//    const filePath = path.resolve(__dirname, 'resources/valid.csv');
//    await fileInput.setInputFiles(filePath);

//    const reasonDropdown = page.getByPlaceholder('Enter a reason');
//    await reasonDropdown.fill(reason);

//    const colSelectDropdown = page.getByRole('combobox');
//    await expect(colSelectDropdown).toBeVisible();
//    await colSelectDropdown.selectOption({ label: "Col-4 - PseudoID" });
//}
