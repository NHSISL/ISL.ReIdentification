import { entraUser } from "../models/views/components/entraUsers/entraUsers";
import ApiBroker from "./apiBroker";

class EntraUsersBroker {
    absoluteUrl = 'https://graph.microsoft.com/v1.0/users';
    scope = 'Directory.Read.All'
    
    private apiBroker: ApiBroker = new ApiBroker(this.scope);

    async FilterUsersAsync(emailAddressFragment: string) {
        if(!emailAddressFragment) {
            return [];
        }

        const url = `${this.absoluteUrl}?$filter=startswith(mail,'${emailAddressFragment}')`;

        return await this.apiBroker.GetAsyncAbsolute(url)
            .then(result => {
                if(result.data && result.data.value) {
                    return result.data.value.map((eu: entraUser) => { return new entraUser(eu) })
                }
            });
    }
}
export default EntraUsersBroker;