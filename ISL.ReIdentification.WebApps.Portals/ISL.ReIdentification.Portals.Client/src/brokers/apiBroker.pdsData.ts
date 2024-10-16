import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { PdsData } from "../models/pds/pdsData";

class PdsDataBroker {
    relativePdsDataUrl = '/api/pdsData';
    relativePdsDataOdataUrl = '/odata/pdsData'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((pdsData: any) => new PdsData(pdsData));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostPdsDataAsync(pdsData: PdsData) {
        return await this.apiBroker.PostAsync(this.relativePdsDataUrl, pdsData)
            .then(result => new PdsData(result.data));
    }

    async GetAllPdsDataAsync(queryString: string) {
        const url = this.relativePdsDataUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((pdsData: any) => new PdsData(pdsData)));
    }

    async GetPdsDataFirstPagesAsync(query: string) {
        const url = this.relativePdsDataOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetPdsDataSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetPdsDataByIdAsync(id: Guid) {
        const url = `${this.relativePdsDataUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new PdsData(result.data));
    }

    async PutPdsDataAsync(pdsData: PdsData) {
        return await this.apiBroker.PutAsync(this.relativePdsDataUrl, pdsData)
            .then(result => new PdsData(result.data));
    }

    async DeletePdsDataByIdAsync(id: Guid) {
        const url = `${this.relativePdsDataUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new PdsData(result.data));
    }
}
export default PdsDataBroker;