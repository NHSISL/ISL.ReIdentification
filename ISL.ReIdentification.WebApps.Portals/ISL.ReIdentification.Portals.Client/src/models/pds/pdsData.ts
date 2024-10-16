import { Guid } from "guid-typescript";

export class PdsData {
    public rowId: Guid;
    public pseudoNhsNumber: string;
    public primaryCareProvider: string;
    public primaryCareProviderBusinessEffectiveFromDate?: Date | undefined;
    public primaryCareProviderBusinessEffectiveToDate?: Date | undefined;
    public ccgOfRegistration: string;
    public currentCcgOfRegistration: string;
    public icbOfRegistration?: string;
    public currentIcbOfRegistration?: string;

    constructor(pds: any) {
        this.rowId = pds.rowId ? Guid.parse(pds.rowId) : Guid.parse(Guid.EMPTY);
        this.pseudoNhsNumber = pds.pseudoNhsNumber || "";
        this.primaryCareProvider = pds.primaryCareProvider || "";

        this.primaryCareProviderBusinessEffectiveFromDate =
            pds.primaryCareProviderBusinessEffectiveFromDate
                ? new Date(pds.primaryCareProviderBusinessEffectiveFromDate)
                : undefined;

        this.primaryCareProviderBusinessEffectiveToDate =
            pds.primaryCareProviderBusinessEffectiveToDate
                ? new Date(pds.primaryCareProviderBusinessEffectiveToDate)
                : undefined;

        this.ccgOfRegistration = pds.ccgOfRegistration || "";
        this.currentCcgOfRegistration = pds.currentCcgOfRegistration || "";
        this.icbOfRegistration = pds.icbOfRegistration || "";
        this.currentIcbOfRegistration = pds.currentIcbOfRegistration || "";
    }
}