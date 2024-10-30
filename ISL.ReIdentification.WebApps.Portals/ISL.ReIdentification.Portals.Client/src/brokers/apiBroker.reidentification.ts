import { AccessRequest } from "../models/accessRequest/accessRequest";
import ApiBroker from "./apiBroker";

class SimpleReIdentificationBroker {
    relativeReIdentificationUrl = '/api/reIdentification/';
    relativeReIdentificationCsvUrl = '/api/reIdentification/submitcsv';
    relativeCsvReIdentificationUrl = '/api/reIdentification/csvreidentification?csvIdentificationRequestId=';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostReIdentificationAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async PostReIdentificationCsvAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationCsvUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async GetCsvIdentificationRequestByIdAsync(id: string) {
        const url = `${this.relativeCsvReIdentificationUrl}${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as AccessRequest);
    }
}

export default SimpleReIdentificationBroker;