import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { ImpersonationContext } from "../models/impersonationContext/impersonationContext";

class ImpersonationContextBroker {
    relativeImpersonationContextUrl = '/api/impersonationContext';
    relativeImpersonationContextOdataUrl = '/odata/impersonationContext'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((impersonationContext: any) => new ImpersonationContext(impersonationContext));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PostAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => new ImpersonationContext(result.data));
    }

    async GetAllImpersonationContextAsync(queryString: string) {
        const url = this.relativeImpersonationContextUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((impersonationContext: any) => new ImpersonationContext(impersonationContext)));
    }

    async GetImpersonationContextFirstPagesAsync(query: string) {
        const url = this.relativeImpersonationContextOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetImpersonationContextSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetImpersonationContextByIdAsync(id: Guid) {
        const url = `${this.relativeImpersonationContextUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new ImpersonationContext(result.data));
    }

    async PutImpersonationContextAsync(impersonationContext: ImpersonationContext) {
        return await this.apiBroker.PutAsync(this.relativeImpersonationContextUrl, impersonationContext)
            .then(result => new ImpersonationContext(result.data));
    }

    async DeleteImpersonationContextByIdAsync(id: Guid) {
        const url = `${this.relativeImpersonationContextUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new ImpersonationContext(result.data));
    }
}
export default ImpersonationContextBroker;