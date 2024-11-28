import { test, expect, Page } from '@playwright/test';
import testDataForDataset from './testData/testDataForDataset.json' assert { type: 'json' };


test.describe('Re-Identify dataset Patient Tests', () => {

    test.beforeEach(async ({ page }) => {
        await page.goto('https://localhost:5173/home');
    });

    testDataForDataset.forEach(({ recipientEmail, reason }) => {
        test(`Can Worklist: ${recipientEmail}`, async ({ page }) => {
            
        });
    });

});