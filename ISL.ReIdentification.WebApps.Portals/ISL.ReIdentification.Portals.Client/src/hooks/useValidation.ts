import { useCallback, useEffect, useState } from "react";
import { ApiValidationError } from "../errors/validationError";
import { Validation } from "../models/validations/validation";
import { ErrorBase } from "../types/ErrorBase";
import { ValidationProcessor } from "./validationProcessor";

export function useValidation<T extends ErrorBase>(errorSpecification: T, validations: Validation[], values: any) {
    const [errors, setErrors] = useState(errorSpecification);
    const [validationEnabled, setValidationEnabled] = useState(false);
    const [hasErrors, setHasErrors] = useState(false);

    const watchValidation = useCallback((values: any) => {
        const validationErrors = ValidationProcessor<T>().validate(errorSpecification, validations, values, validationEnabled)
        setErrors(validationErrors);
        setHasErrors(validationErrors.hasErrors);
        return validationErrors.hasErrors;
    }, [validations, errorSpecification, validationEnabled]);

    const processValidation = useCallback((values: any) => {
        const validationErrors = ValidationProcessor<T>().validate(errorSpecification, validations, values, true)
        setErrors(validationErrors);
        setHasErrors(validationErrors.hasErrors);
        return validationErrors.hasErrors;
    }, [validations, errorSpecification]);

    useEffect(() => {
        if (!validationEnabled) {
            setErrors(errorSpecification);
            return;
        }
        watchValidation(values);
    }, [values, watchValidation, validationEnabled, errorSpecification])

    const processApiErrors = useCallback((apiErrors: ApiValidationError) => {
        const processedErrors = ValidationProcessor().processApiErrors(apiErrors, errorSpecification);
        setHasErrors(processedErrors.hasErrors);
        if(processedErrors.hasErrors) {
            setErrors(processedErrors.errors);
        } else {
            setErrors(errorSpecification);
        }
    }, [errorSpecification])

    const enableValidationMessages = () => {
        setValidationEnabled(true)
    }

    const disableValidationMessages = () => {
        setValidationEnabled(false)
    }

    return {
        errors, hasErrors, processApiErrors, enableValidationMessages, disableValidationMessages, validate: processValidation
    };
}
