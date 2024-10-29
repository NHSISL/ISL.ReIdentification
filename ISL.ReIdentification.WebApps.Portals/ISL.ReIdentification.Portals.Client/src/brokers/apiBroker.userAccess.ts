import ApiBroker from "./apiBroker";
import { UserAccess } from "../models/userAccess/userAccess";

class UserAccessBroker {
    relativeUserAccessUrl = '/odata/userAccesses';

    private apiBroker: ApiBroker = new ApiBroker();

    async FilterUserAccessAsync(userString: string) {
        if (!userString) {
            return [];
        }

        const url = `${this.relativeUserAccessUrl}?$filter=startswith(recipientEmail,'${userString}')`;

        return await this.apiBroker.GetAsync(url)
            .then(result => {
                if (result.data) {
                    return result.data as UserAccess[];
                }
            })
    }
}
export default UserAccessBroker;