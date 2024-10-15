import { Guid } from "guid-typescript";

export class PdsDataView {
    public rowId: Guid;
    public pseudoNhsNumber: string;
    public primaryCareProvider: string;
    public primaryCareProviderBusinessEffectiveFromDate?: Date | undefined;
    public primaryCareProviderBusinessEffectiveToDate?: Date | undefined;
    public ccgOfRegistration: string;
    public currentCcgOfRegistration: string;
    public icbOfRegistration?: string;
    public currentIcbOfRegistration?: string;

    constructor(
        rowId: Guid,
        pseudoNhsNumber: string,
        primaryCareProvider: string,
        primaryCareProviderBusinessEffectiveFromDate?: Date,
        primaryCareProviderBusinessEffectiveToDate?: Date,
        ccgOfRegistration: string,
        currentCcgOfRegistration: string,
        icbOfRegistration?: string,
        currentIcbOfRegistration?: string;
    ) {
        this.rowId = rowId;
        this.pseudoNhsNumber = pseudoNhsNumber;
        this.primaryCareProvider = primaryCareProvider;
        this.primaryCareProviderBusinessEffectiveFromDate = primaryCareProviderBusinessEffectiveFromDate
        this.primaryCareProviderBusinessEffectiveToDate = primaryCareProviderBusinessEffectiveToDate
        this.ccgOfRegistration = ccgOfRegistration;
        this.currentCcgOfRegistration = currentCcgOfRegistration;
        this.icbOfRegistration = icbOfRegistration;
        this.currentIcbOfRegistration = currentIcbOfRegistration;
    }
}