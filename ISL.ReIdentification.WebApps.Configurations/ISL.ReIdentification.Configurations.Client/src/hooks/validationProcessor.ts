import { ApiValidationError } from "../errors/validationError";
import { Validation } from "../models/validations/validation";
import { ValidationMessages } from "./validationMessages";

export function ValidationProcessor<T>() {

    const processRequired = (key: string, values: any, isRequired?: boolean) => {
        if (!isRequired) {
            return;
        }

        if (!values[key]) {
            return ValidationMessages.requiredMessage();
        }
    }

    const processMinLength = (key: string, values: any, minlength?: number) => {
        if (!minlength) {
            return;
        }

        if (minlength > values[key].length) {
            return ValidationMessages.minimumLengthMessage(minlength);
        }
    }

    const processMaxLength = (key: string, values: any, maxlength?: number) => {
        if (!maxlength) {
            return;
        }

        if (maxlength < values[key].length) {
            return ValidationMessages.maximumLengthMessage(maxlength);
        }
    }

    const processMinValue = (key: string, values: any, minValue?: number) => {
        if (!minValue) {
            return;
        }

        if (minValue > values[key]) {
            return ValidationMessages.minimumValueMessage(minValue);
        }
    }

    const processMaxValue = (key: string, values: any, maxValue?: number) => {
        if (!maxValue) {
            return;
        }

        if (maxValue < values[key]) {
            return ValidationMessages.maximumValueMessage(maxValue);
        }
    }

    const processMinDate = (key: string, values: any, minDate?: Date) => {
        if (!minDate) {
            return;
        }

        if (minDate > values[key]) {
            return ValidationMessages.minimumDateMessage(minDate);
        }
    }

    const processMaxDate = (key: string, values: any, maxDate?: Date) => {
        if (!maxDate) {
            return;
        }

        if (maxDate < values[key]) {
            return ValidationMessages.maximumDateMessage(maxDate);
        }
    }

    const processEnumValidation = (key: string, values: any, message?: string, regex?: string | RegExp) => {
        if (!regex) {
            return;
        }

        var reg = new RegExp(regex);

        if (reg.test(values[key]) === false) {
            return ValidationMessages.regExFail();;
        }
    }

    const processValidation = (errorSpecification: T, validations: Validation[], values: any, validationEnabled: boolean) : T => {
        let validationErrors: any = { ...errorSpecification };
        
        validations.forEach((validation: Validation) => {
            let propertyErrors: Array<any> = [];

            propertyErrors.push(processRequired(validation.property, values, validation.isRequired));
            propertyErrors.push(processMinLength(validation.property, values, validation.minLength));
            propertyErrors.push(processMaxLength(validation.property, values, validation.maxLength));
            propertyErrors.push(processMinValue(validation.property, values, validation.minValue));
            propertyErrors.push(processMaxValue(validation.property, values, validation.maxValue));
            propertyErrors.push(processMinDate(validation.property, values, validation.minDate));
            propertyErrors.push(processMaxDate(validation.property, values, validation.maxDate));
            propertyErrors.push(processEnumValidation(validation.property, values, validation.errorMessage, validation.regex));

            const processedError = propertyErrors.filter((x: any) => x);

            if (processedError.length) {
                validationErrors[validation.property as keyof T] = `${validation.friendlyName} ${processedError.join(" and ")}.`;
                validationErrors.hasErrors = true;
            }
        })

        if(validationEnabled) {
            return validationErrors;
        } 

        return errorSpecification;
    };

    const processApiErrors = (apiErrors: ApiValidationError, errorSpecification: any) => {
        let errors: any = { ...errorSpecification };
        for (const key in apiErrors.errors) {
            errors[key] = apiErrors.errors[key].join(", ");
        }
        if (apiErrors.errors) {
            return {hasErrors: true, errors}
        }
        return {hasErrors:false, errors: null}
    }

    return {
        processApiErrors, validate: processValidation
    };
}
