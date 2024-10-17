export class IdentificationItem {
    public rowNumber: string;
    public identifier: string;
    public message: string;
    public hasAccess?: boolean;
    public isReidentified?: boolean;

    constructor(identificationItem: IdentificationItem) {
        this.rowNumber = identificationItem.rowNumber ? identificationItem.rowNumber : "";
        this.identifier = identificationItem.identifier || "";
        this.message = identificationItem.message || "";
        this.hasAccess = identificationItem.hasAccess || false;
        this.isReidentified = identificationItem.isReidentified || false;
    }
}