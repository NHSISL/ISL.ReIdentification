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

    async GetCsvIdentificationRequestByIdAsync(id: string): Promise<AccessRequest> {
        const url = `${this.relativeCsvReIdentificationUrl}${id}`;

        try {
            const result = await this.apiBroker.GetAsync(url);
            return result.data as AccessRequest;
        } catch (error) {
            console.error("Error fetching CSV Identification Request by ID:", error);
            throw error;
        }
    }
}

export default SimpleReIdentificationBroker;