import { userAccessService } from "../../foundations/userAccessService";

export const userAccessViewService = {
    useCreateUserAccess: () => {
        return userAccessService.useCreateUserAccess();
    },

    useGetDistinctUsers: () => {
        const response = userAccessService.useRetrieveDistinctUsers();
        return {...response}
    },

    useGetAccessForUser: (entraId?: string) => {
        const query = `?$filter=entraUserId eq ${entraId}`;
        return userAccessService.useRetrieveAllUserAccess(query);
    },
};