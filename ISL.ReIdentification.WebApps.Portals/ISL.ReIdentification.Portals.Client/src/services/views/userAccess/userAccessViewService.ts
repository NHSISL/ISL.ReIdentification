import { userAccessService } from "../../foundations/userAccessService";

export const userAccessViewService = {
    useGetAccessForUser: (entraId?: string) => {
        const query = `?$filter=entraUserId eq '${entraId}'`;
        return userAccessService.useGetAllUserAccess(query);
    },
    useGetOrgCodeAccessForUser: (entraId?: string) => {
        const query = `?$filter=entraUserId eq '${entraId}'&$select=orgCode`;
        return userAccessService.useGetAllUserAccess(query);
    }
}