import { CsvIdentificationRequest } from "../../../csvIdentificationRequest/csvIdentificationRequest";
import { ImpersonationContext } from "../../../impersonationContext/impersonationContext";
import { IdentificationRequest } from "../../../ReIdentifications/IdentificationRequest";

export class AccessRequestView {
    public identificationRequest?: IdentificationRequest;
    public csvIdentificationRequest?: CsvIdentificationRequest;
    public impersonationContext?: ImpersonationContext;

    constructor(
        identificationRequest?: IdentificationRequest,
        csvIdentificationRequest?: CsvIdentificationRequest,
        impersonationContext?: ImpersonationContext
    ) {
        this.identificationRequest = identificationRequest;
        this.csvIdentificationRequest = csvIdentificationRequest;
        this.impersonationContext = impersonationContext;
    }
}