import { IdentificationItem } from './IdentificationItem';

export class IdentificationRequest {
    public id: string;
    public identificationItems: IdentificationItem[];
    public entraUserId: string;
    public givenName: string;
    public surname: string;
    public displayName: string;
    public jobTitle: string;
    public email: string;
    public purpose: string;
    public organisation: string;
    public reason: string;

    constructor(identificationRequest: IdentificationRequest) {
        this.id = identificationRequest.id || "";
        this.identificationItems = identificationRequest.identificationItems || [];
        this.entraUserId = identificationRequest.entraUserId || "";
        this.givenName = identificationRequest.givenName || "";
        this.surname = identificationRequest.surname || "";
        this.displayName = identificationRequest.displayName || "";
        this.jobTitle = identificationRequest.jobTitle || "";
        this.email = identificationRequest.email || "";
        this.purpose = identificationRequest.purpose || "";
        this.organisation = identificationRequest.organisation || "";
        this.reason = identificationRequest.reason || "";
    }
}
