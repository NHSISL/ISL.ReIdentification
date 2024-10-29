import { AccessRequest } from "../models/accessRequest/accessRequest";
import ApiBroker from "./apiBroker";

class SimpleReIdentificationBroker {
    relativeReIdentificationUrl = '/api/reIdentification/';
    relativeReIdentificationCsvUrl = '/api/reIdentification/submitcsv';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostReIdentificationAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async PostReIdentificationCsvAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationCsvUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }
}

export default SimpleReIdentificationBroker;