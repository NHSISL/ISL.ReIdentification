import { AxiosResponse } from "axios";
import { Guid } from "guid-typescript";
import ApiBroker from "./apiBroker";
import { UserAccess } from "../models/userAccess/userAccess";

class UserAccessBroker {
    relativeUserAccessUrl = '/api/userAccesses';
    relativeUserAccessOdataUrl = '/odata/userAccesses'

    private apiBroker: ApiBroker = new ApiBroker();

    private processOdataResult = (result: AxiosResponse) => {
        const data = result.data.value.map((userAccess: any) => new UserAccess(userAccess));

        const nextPage = result.data['@odata.nextLink'];
        return { data, nextPage }
    }

    async PostUserAccessAsync(userAccess: UserAccess) {
        return await this.apiBroker.PostAsync(this.relativeUserAccessUrl, userAccess)
            .then(result => new UserAccess(result.data));
    }

    async GetAllUserAccessAsync(queryString: string) {
        const url = this.relativeUserAccessUrl + queryString;

        return await this.apiBroker.GetAsync(url)
            .then(result => result.data.map((userAccess: any) => new UserAccess(userAccess)));
    }

    async GetUserAccessFirstPagesAsync(query: string) {
        const url = this.relativeUserAccessOdataUrl + query;
        return this.processOdataResult(await this.apiBroker.GetAsync(url));
    }

    async GetUserAccessSubsequentPagesAsync(absoluteUri: string) {
        return this.processOdataResult(await this.apiBroker.GetAsyncAbsolute(absoluteUri));
    }

    async GetLookupByIdAsync(id: Guid) {
        const url = `${this.relativeUserAccessUrl}/${id}`;

        return await this.apiBroker.GetAsync(url)
            .then(result => new UserAccess(result.data));
    }

    async PutUserAccessAsync(userAccess: UserAccess) {
        return await this.apiBroker.PutAsync(this.relativeUserAccessUrl, userAccess)
            .then(result => new UserAccess(result.data));
    }

    async DeleteUserAccessByIdAsync(id: Guid) {
        const url = `${this.relativeUserAccessUrl}/${id}`;

        return await this.apiBroker.DeleteAsync(url)
            .then(result => new UserAccess(result.data));
    }
}
export default UserAccessBroker;