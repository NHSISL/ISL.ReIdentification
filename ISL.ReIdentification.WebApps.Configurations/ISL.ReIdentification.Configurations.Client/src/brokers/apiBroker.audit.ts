import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { Audit } from "../models/audit/audit";

export type AuditOdataResponse = {
    data: Audit[],
    nextPage: string
}

class AuditBroker {
    relativeAuditUrl = '/api/audits';
    relativeAuditOdataUrl = '/odata/audits'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse): AuditOdataResponse => {
        const data = result.data.value as Audit[];

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async GetAllAuditAsync(queryString: string) {
        const url = this.relativeAuditUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as Audit[]);
    }

    async GetAuditFirstPagesAsync(query: string) {
        const url = this.relativeAuditOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetAuditSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetAuditByAuditTypeAsync(auditType: string) {
        const url = `${this.relativeAuditUrl}/type/${auditType}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as Audit);
    }
}
export default AuditBroker;