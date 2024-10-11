import { useMsal } from "@azure/msal-react";
import { Guid } from "guid-typescript";
import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from "react-query";
import ImpersonationContextBroker from "../../brokers/apiBroker.ImpersonationContext";
import { ImpersonationContext } from "../../models/impersonationContext.ts/impersonationContext";


export const impersonationContextService = {
    useCreateimpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation((impersonationContext: ImpersonationContext) => {
            const date = new Date();
            impersonationContext.createdDate = impersonationContext.updatedDate = date;
            impersonationContext.createdBy = impersonationContext.updatedBy = msal.accounts[0].username;

            return broker.PostImpersonationContextAsync(impersonationContext);
        },
            {
                onSuccess: (variables: ImpersonationContext) => {
                    queryClient.invalidateQueries("ImpersonationContextGetAll");
                    queryClient.invalidateQueries(["ImpersonationContextById", { id: variables.id }]);
                }
            });
    },

    useRetrieveAllImpersonationContext: (query: string) => {
        const broker = new ImpersonationContextBroker();

        return useQuery(
            ["ImpersonationContextGetAll", { query: query }],
            () => broker.GetAllImpersonationContextAsync(query),
            { staleTime: Infinity });
    },

    useRetrieveAllImpersonationContextPages: (query: string) => {
        const broker = new ImpersonationContextBroker();

        return useInfiniteQuery(
            ["ImpersonationContextGetAll", { query: query }],
            ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetImpersonationContextFirstPagesAsync(query)
                }
                return broker.GetImpersonationContextSubsequentPagesAsync(pageParam)
            },
            {
                getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
                staleTime: Infinity
            });
    },

    useModifyImpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation((impersonationContext: ImpersonationContext) => {
            const date = new Date();
            impersonationContext.updatedDate = date;
            impersonationContext.updatedBy = msal.accounts[0].username;

            return broker.PutImpersonationContextAsync(impersonationContext);
        },
            {
                onSuccess: (data: ImpersonationContext) => {
                    queryClient.invalidateQueries("ImpersonationContextGetAll");
                    queryClient.invalidateQueries(["ImpersonationContextGetById", { id: data.id }]);
                }
            });
    },

    useRemoveImpersonationContext: () => {
        const broker = new ImpersonationContextBroker();
        const queryClient = useQueryClient();

        return useMutation((id: Guid) => {
            return broker.DeleteImpersonationContextByIdAsync(id);
        },
            {
                onSuccess: (data: { id: Guid }) => {
                    queryClient.invalidateQueries("ImpersonationContextGetAll");
                    queryClient.invalidateQueries(["ImpersonationContextGetById", { id: data.id }]);
                }
            });
    },
}