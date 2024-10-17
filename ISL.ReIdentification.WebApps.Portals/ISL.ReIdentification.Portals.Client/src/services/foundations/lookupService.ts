import { useMsal } from "@azure/msal-react";
import { Guid } from "guid-typescript";
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import LookupBroker from "../../brokers/apiBroker.lookups";
import { Lookup } from "../../models/lookups/lookup";

export const lookupService = {
    useCreatelookup: () => {
        const broker = new LookupBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (lookup: Lookup) => {
                const date = new Date();
                lookup.createdDate = lookup.updatedDate = date;
                lookup.createdBy = lookup.updatedBy = msal.accounts[0].username;

                return broker.PostLookupAsync(lookup);
            },
            onSuccess: (variables: Lookup) => {
                queryClient.invalidateQueries({ queryKey: ["LookupGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["LookGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllLookups: (query: string) => {
        const broker = new LookupBroker();

        return useQuery({
            queryKey: ["LookupGetAll", { query: query }],
            queryFn: () => broker.GetAllLookupsAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllLookupPages: (query: string) => {
        const lookupBroker = new LookupBroker();

        return useQuery({
            queryKey: ["LookupGetAll", { query: query }],
            queryFn: () => lookupBroker.GetAllLookupsAsync(query),
            staleTime: Infinity
        });
    },

    useModifyLookup: () => {
        const lookupBroker = new LookupBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation({
            mutationFn: (lookup: Lookup) => {
                const date = new Date();
                lookup.updatedDate = date;
                lookup.updatedBy = msal.accounts[0].username;

                return lookupBroker.PutLookupAsync(lookup);
            },

            onSuccess: (data: { id: any }) => {
                queryClient.invalidateQueries({ queryKey: ["LookupGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["LookupGetById", { id: data.id }] });
            }
        });
    },

    useRemoveLookup: () => {
        const broker = new LookupBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: Guid) => {
                return broker.DeleteLookupByIdAsync(id);
            },
            onSuccess: (data: { id: Guid }) => {
                queryClient.invalidateQueries({ queryKey: ["LookupGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["LookupGetById", { id: data.id }] });
            }
        });
    },
}