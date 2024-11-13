import { userAccessService } from "../../foundations/userAccessService";

export const userAccessViewService = {
    useGetAccessForUser: (entraId?: string) => {
        const query = `?$filter=entraUserId eq ${entraId}`;
        return userAccessService.useGetAllUserAccess(query);
    }

}