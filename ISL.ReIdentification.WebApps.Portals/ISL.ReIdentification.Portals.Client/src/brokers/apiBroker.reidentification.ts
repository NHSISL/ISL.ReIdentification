import ApiBroker from "./apiBroker";
import { AccessRequestView } from "../models/views/components/accessRequest/accessRequestView";
import { AccessRequest } from "../models/accessRequest/accessRequest";

class ReIdentificationBroker {
    relativeReIdentificationUrl = '/api/reIdentification';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostReIdentificationAsync(accessRequestView: AccessRequestView) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationUrl, accessRequestView)
            .then(result => new AccessRequest(result.data));
    }
}

export default ReIdentificationBroker;