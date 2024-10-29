import ApiBroker from "./apiBroker";
import { UserAccess } from "../models/userAccess/userAccess";

class UserAccessBroker {
    relativeUserAccessUrl = '/odata/userAccesses';

    private apiBroker: ApiBroker = new ApiBroker();

    async FilterUserAccessAsync(userString: string) {
        if (!userString) {
            return [];
        }

        const url = `${this.relativeUserAccessUrl}?$filter=startswith(email,'${userString}')`;

        return await this.apiBroker.GetAsync(url)
            .then(result => {
                if (result.data && result.data.value) {
                    return result.data.value as UserAccess[];
                }
            })
    }
}
export default UserAccessBroker;