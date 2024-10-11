import { useEffect, useState } from "react";
import { Guid } from "guid-typescript";
import { UserAccessView } from "../../../models/views/components/userAccess/userAccessView";
import { userAccessService } from "../../foundations/userAccessService";
import { UserAccess } from "../../../models/userAccess/userAccess";

type UserAccessViewServiceResponse = {
    mappedUserAccess: UserAccessView[] | undefined;
    pages: Array<{ data: UserAccessView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: UserAccessView[] }> } | undefined;
    refetch: () => void;
};

export const userAccessViewService = {
    useCreateUserAccess: () => {
        return userAccessService.useCreateUserAccess();
    },

    useGetAllUserAccess: (searchTerm?: string): UserAccessViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(UserEmail,'${searchTerm}') or 
                 contains(firstName,'${searchTerm}') or 
                 contains(lastName,'${searchTerm}') or 
                 contains(orgCode,'${searchTerm}')`;
        }

        const response = userAccessService.useRetrieveAllUserAccessPages(query);
        const [mappedUserAccess, setMappedUserAccess] = useState<Array<UserAccessView>>();
        const [pages, setPages] = useState<any>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const userAccesses: Array<UserAccessView> = [];
                response.data.pages.forEach(page => {
                    page.data.forEach((userAccess: UserAccess) => {
                        userAccesses.push(new UserAccessView(
                            userAccess.id,
                            userAccess.firstName,
                            userAccess.lastName,
                            userAccess.userEmail,
                            userAccess.orgCode,
                            userAccess.activeFrom,
                            userAccess.activeTo,
                            userAccess.createdBy,
                            userAccess.createdDate,
                            userAccess.updatedBy,
                            userAccess.updatedDate,
                        ));
                    });
                });

                setMappedUserAccess(userAccesses);
                setPages(response.data.pages);
            }
        }, [response.data?.pages]);

        return {
            mappedUserAccess,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetUserAccessById: (id: string) => {
        const query = `?$filter=id eq ${id}`;
        const response = userAccessService.useRetrieveAllUserAccessPages(query);
        const [mappedUserAccess, setMappedUserAccess] = useState<UserAccessView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const userAccess = response.data.pages[0].data[0];
                const userAccessView = new UserAccessView(
                    userAccess.id,
                    userAccess.firstName,
                    userAccess.lastName,
                    userAccess.userEmail,
                    userAccess.orgCode,
                    userAccess.activeFrom,
                    userAccess.activeTo,
                    userAccess.createdBy,
                    userAccess.createdDate,
                    userAccess.updatedBy,
                    userAccess.updatedDate
                );

                setMappedUserAccess(userAccessView);
            }
        }, [response.data?.pages]);

        return {
            mappedUserAccess,
            ...response
        };
    },

    useUpdateLookup: () => {
        return userAccessService.useModifyUserAccess();
    },

    useRemoveLookup: () => {
        return userAccessService.useRemoveUserAccess();
    },
};