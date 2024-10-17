import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { OdsData } from "../models/odsData/odsData";

class OdsDataBroker {
    relativeOdsDataUrl = '/api/odsData';
    relativeOdsDataOdataUrl = '/odata/odsData'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((odsData: any) => new OdsData(odsData));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostOdsDataAsync(odsData: OdsData) {
        return await this.apiBroker.PostAsync(this.relativeOdsDataUrl, odsData)
            .then(result => new OdsData(result.data));
    }

    async GetAllOdsDataAsync(queryString: string) {
        const url = this.relativeOdsDataUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((odsData: any) => new OdsData(odsData)));
    }

    async GetOdsDataFirstPagesAsync(query: string) {
        const url = this.relativeOdsDataOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetOdsDataSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetOdsDataByIdAsync(id: Guid) {
        const url = `${this.relativeOdsDataUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new OdsData(result.data));
    }

    async PutOdsDataAsync(odsData: OdsData) {
        return await this.apiBroker.PutAsync(this.relativeOdsDataUrl, odsData)
            .then(result => new OdsData(result.data));
    }

    async DeleteOdsDataByIdAsync(id: Guid) {
        const url = `${this.relativeOdsDataUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new OdsData(result.data));
    }
}
export default OdsDataBroker;