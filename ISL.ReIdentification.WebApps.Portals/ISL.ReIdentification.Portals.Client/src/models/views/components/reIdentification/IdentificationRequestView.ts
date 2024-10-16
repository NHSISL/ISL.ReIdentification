import { IdentificationItemView } from "./IdentificationItemView";

export class IdentificationRequestView {
    public id: string;
    public identificationItems: IdentificationItemView[];
    public entraUserId: string;
    public givenName: string;
    public surname: string;
    public displayName: string;
    public jobTitle: string;
    public email: string;
    public purpose: string;
    public organisation: string;
    public reason: string;

    constructor(
        id: string,
        identificationItems: IdentificationItemView[],
        entraUserId: string,
        givenName: string,
        surname: string,
        displayName: string,
        jobTitle: string,
        email: string,
        purpose: string,
        organisation: string,
        reason: string
    ) {
        this.id = id || "";
        this.identificationItems = identificationItems || [];
        this.entraUserId = entraUserId || "";
        this.givenName = givenName || "";
        this.surname = surname || "";
        this.displayName = displayName || "";
        this.jobTitle = jobTitle || "";
        this.email = email || "";
        this.purpose = purpose || "";
        this.organisation = organisation || "";
        this.reason = reason || "";
    }
}
