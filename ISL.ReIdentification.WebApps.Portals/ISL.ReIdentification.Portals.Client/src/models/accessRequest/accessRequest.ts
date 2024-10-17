import { CsvIdentificationRequest } from "../csvIdentificationRequest/csvIdentificationRequest";
import { ImpersonationContext } from "../impersonationContext/impersonationContext";
import { IdentificationRequest } from "../ReIdentifications/IdentificationRequest";

export class AccessRequest {
    public identificationRequest: IdentificationRequest;
    public csvIdentificationRequest?: CsvIdentificationRequest;
    public impersonationContext?: ImpersonationContext;

    constructor(accessRequest: AccessRequest) {
        this.identificationRequest = accessRequest.identificationRequest || {};
        this.csvIdentificationRequest = accessRequest.csvIdentificationRequest || {};
        this.impersonationContext = accessRequest.impersonationContext || {};
    }
}