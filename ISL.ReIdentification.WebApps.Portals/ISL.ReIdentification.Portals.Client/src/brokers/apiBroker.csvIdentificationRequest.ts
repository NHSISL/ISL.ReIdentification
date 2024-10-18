import ApiBroker from "./apiBroker";
import { AccessRequestView } from "../models/views/components/accessRequest/accessRequestView";
import { AccessRequest } from "../models/accessRequest/accessRequest";

class CsvIdentificationRequestBroker {
    relativeCsvReIdentificationUrl = '/api/reIdentification/submitcsv';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostCsvReIdentificationAsync(accessRequestView: AccessRequestView) {
        return await this.apiBroker.PostAsync(this.relativeCsvReIdentificationUrl, accessRequestView)
            .then(result => new AccessRequest(result.data));
    }
}

export default CsvIdentificationRequestBroker;