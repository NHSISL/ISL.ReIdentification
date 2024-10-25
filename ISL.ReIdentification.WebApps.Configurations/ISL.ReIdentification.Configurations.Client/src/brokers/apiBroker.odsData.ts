import { AxiosResponse } from "axios";
import ApiBroker from "./apiBroker";
import { OdsData } from "../models/odsData/odsData";

type OdsODataResponse = {
    data : OdsData[],
    nextPage: string
}

class OdsDataBroker {
    relativeOdsDataUrl = '/api/odsData';
    relativeOdsDataOdataUrl = '/odata/odsData'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) : OdsODataResponse => {
        const data = result.data.value.map((odsData: OdsData) => new OdsData(odsData));

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
            .then(result => result.data.map((odsData: OdsData) => new OdsData(odsData)));
    }

    async GetOdsDataFirstPagesAsync(query: string) {
        const url = this.relativeOdsDataOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetOdsDataSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetOdsDataByIdAsync(id: string) {
        const url = `${this.relativeOdsDataUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new OdsData(result.data));
    }

    async GetOdsChildrenByIdAsync(id: string) {
        const url = `${this.relativeOdsDataUrl}/GetChildren?id=${id}`;
        
        if(!id) {
            return [];
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((odsData: OdsData) => new OdsData(odsData)));
    }

    async PutOdsDataAsync(odsData: OdsData) {
        return await this.apiBroker.PutAsync(this.relativeOdsDataUrl, odsData)
            .then(result => new OdsData(result.data));
    }

    async DeleteOdsDataByIdAsync(id: string) {
        const url = `${this.relativeOdsDataUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new OdsData(result.data));
    }
}
export default OdsDataBroker;