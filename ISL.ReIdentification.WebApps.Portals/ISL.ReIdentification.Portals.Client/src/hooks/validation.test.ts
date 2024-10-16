import { Validation } from "../models/validations/validation";
import { it } from '@jest/globals'
import "@testing-library/jest-dom/extend-expect";
import { ValidationProcessor } from "./validationProcessor";
import { ApiValidationError } from "../errors/validationError";
import { ValidationMessages } from "./validationMessages";

type TestErrors = {
    test: string
}

const valuesDefinition = {
    test: ""
}

const errorObject: TestErrors = {
    test: ""
}

let container: any;

beforeEach(() => {
    container = document.createElement('span');
});

afterEach(() => {
    container.remove();
    container = null;
});

it("Disabled validation returns no message", () => {
    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        isRequired: true
    }]

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, valuesDefinition, false);

    expect(validationErrors.test).toBe("");
})

it("No validation requirement returns empty message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        isRequired: false
    }]

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, valuesDefinition, true);

    expect(validationErrors.test).toBe("");
})

it("Is Required validation returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        isRequired: true
    }]

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, valuesDefinition, true);

    expect(validationErrors.test).not.toBe(null);
    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.requiredMessage()}.`);
})

it("Min length validation returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        minLength: 2
    }]

    const values = { ...valuesDefinition, test: "1" };

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, values, true);

    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.minimumLengthMessage(2)}.`);
})

it("Max length validation returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        maxLength: 2
    }]

    const values = { ...valuesDefinition, test: "123" };

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, values, true);

    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.maximumLengthMessage(2)}.`);
})

it("Min value validation returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        minValue: 2
    }]

    const values = { ...valuesDefinition, test: 1 };

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, values, true);

    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.minimumValueMessage(2)}.`);
})

it("Min value validation returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        maxValue: 2
    }]

    const values = { ...valuesDefinition, test: 3 };

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, values, true);

    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.maximumValueMessage(2)}.`);
})


it("Multiple validation errors returns correct message", async () => {

    const validation: Validation[] = [{
        friendlyName: Math.random().toString().substr(2, 8),
        property: "test",
        isRequired: true,
        minLength: 3
    }]

    const values = { ...valuesDefinition };

    let validationErrors = ValidationProcessor<TestErrors>().validate(errorObject, validation, values, true);

    expect(validationErrors.test).toBe(`${validation[0].friendlyName} ${ValidationMessages.requiredMessage()} and ${ValidationMessages.minimumLengthMessage(3)}.`);
})

it("API error object is processed correctly", async () => {
    const apiErrors: ApiValidationError = {
        name: "test",
        message: "test",
        errors: {
            "test": ["Error Message 1"]
        }
    }

    let validationErrors = ValidationProcessor<TestErrors>().processApiErrors(apiErrors, errorObject);
    expect(validationErrors.hasErrors).toBe(true);
    expect(validationErrors.errors.test).toBe(apiErrors.errors.test[0])
})

it("API complex error object is processed correctly", async () => {
    const apiErrors: ApiValidationError = {
        name: "test",
        message: "test",
        errors: {
            "test": ["Error Message 1", "Error Message 2"],
            "test2": ["Error Message 3", "Error Message 4"] 
        }
    }

    let validationErrors = ValidationProcessor<TestErrors>().processApiErrors(apiErrors, errorObject);
    expect(validationErrors.hasErrors).toBe(true);
    expect(validationErrors.errors.test).toBe(`${apiErrors.errors.test[0]}, ${apiErrors.errors.test[1]}`)
    expect(validationErrors.errors.test2).toBe(`${apiErrors.errors.test2[0]}, ${apiErrors.errors.test2[1]}`)
})