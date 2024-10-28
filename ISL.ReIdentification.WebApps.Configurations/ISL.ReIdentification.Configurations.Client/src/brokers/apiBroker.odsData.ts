import ApiBroker from "./apiBroker";
import { OdsData } from "../models/odsData/odsData";

class OdsDataBroker {
    relativeOdsDataUrl = '/api/odsData';
    relativeOdsDataOdataUrl = '/odata/odsData'

    private apiBroker: ApiBroker = new ApiBroker();

    async GetAllOdsDataAsync(queryString: string) {
        const url = this.relativeOdsDataUrl + queryString;

        if(!queryString) {
            return []
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as OdsData[]);
    }

    async GetOdsChildrenByIdAsync(id: string) {
        const url = `${this.relativeOdsDataUrl}/GetChildren?id=${id}`;
        
        if(!id) {
            return [];
        }

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data as OdsData[]);
    }
}
export default OdsDataBroker;