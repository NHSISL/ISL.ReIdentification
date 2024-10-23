export class ApiValidationError extends Error {
    errors: any
    
    constructor(validationErrors : Array<any>){
        super("ApiValidationError");
        this.errors = validationErrors
    }
}