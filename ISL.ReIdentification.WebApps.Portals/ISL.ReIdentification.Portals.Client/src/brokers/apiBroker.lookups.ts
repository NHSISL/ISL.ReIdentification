import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { Lookup } from "../models/lookups/lookup";

class LookupBroker {
    relativeLookupUrl = '/api/lookups';
    relativeLookupOdataUrl = '/odata/lookups'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((lookup: any) => new Lookup(lookup));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostLookupAsync(lookup: Lookup) {
        return await this.apiBroker.PostAsync(this.relativeLookupUrl, lookup)
            .then(result => new Lookup(result.data));
    }

    async GetAllLookupsAsync(queryString: string) {
        var url = this.relativeLookupUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((lookup: any) => new Lookup(lookup)));
    }

    async GetLookupFirstPagesAsync(query: string) {
        var url = this.relativeLookupOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetLookupSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetLookupByIdAsync(id: Guid) {
        const url = `${this.relativeLookupUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new Lookup(result.data));
    }

    async PutLookupAsync(lookup: Lookup) {
        return await this.apiBroker.PutAsync(this.relativeLookupUrl, lookup)
            .then(result => new Lookup(result.data));
    }

    async DeleteLookupByIdAsync(id: Guid) {
        const url = `${this.relativeLookupUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new Lookup(result.data));
    }
}
export default LookupBroker;