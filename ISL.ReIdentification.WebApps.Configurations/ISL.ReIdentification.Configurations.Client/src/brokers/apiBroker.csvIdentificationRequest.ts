import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { CsvIdentificationRequest } from "../models/csvIdentificationRequest/csvIdentificationRequest";

class CsvIdentificationRequestBroker {
    relativeCsvIdentificationRequestUrl = '/api/csvIdentificationRequests';
    relativeCsvIdentificationRequestOdataUrl = '/odata/csvIdentificationRequests'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((csvIdentificationRequest: any) => new CsvIdentificationRequest(csvIdentificationRequest));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostCsvIdentificationRequestAsync(csvIdentificationRequest: CsvIdentificationRequest) {
        return await this.apiBroker.PostAsync(this.relativeCsvIdentificationRequestUrl, csvIdentificationRequest)
            .then(result => new CsvIdentificationRequest(result.data));
    }

    async GetAllCsvIdentificationRequestAsync(queryString: string) {
        const url = this.relativeCsvIdentificationRequestUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((csvIdentificationRequest: any) => new CsvIdentificationRequest(csvIdentificationRequest)));
    }

    async GetCsvIdentificationRequestFirstPagesAsync(query: string) {
        const url = this.relativeCsvIdentificationRequestOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetCsvIdentificationRequestSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetCsvIdentificationRequestByIdAsync(id: Guid) {
        const url = `${this.relativeCsvIdentificationRequestUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new CsvIdentificationRequest(result.data));
    }

    async PutCsvIdentificationRequestAsync(csvIdentificationRequest: CsvIdentificationRequest) {
        return await this.apiBroker.PutAsync(this.relativeCsvIdentificationRequestUrl, csvIdentificationRequest)
            .then(result => new CsvIdentificationRequest(result.data));
    }

    async DeleteCsvIdentificationRequestByIdAsync(id: Guid) {
        const url = `${this.relativeCsvIdentificationRequestUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new CsvIdentificationRequest(result.data));
    }
}
export default CsvIdentificationRequestBroker;