import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import OdsDataBroker from "../../brokers/apiBroker.odsData";
import { OdsData } from "../../models/odsData/odsData";

export const odsDataService = {
    useCreateOdsData: () => {
        const broker = new OdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (odsData: OdsData) => {
                return broker.PostOdsDataAsync(odsData);
            },
            onSuccess: (variables: OdsData) => {
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetById", { id: variables.id }] });
            }
        });
    },

    useRetrieveAllOdsData: (query: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsDataGetAll", { query: query }],
            queryFn: () => broker.GetAllOdsDataAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllOdsDataPages: (query: string) => {
        const broker = new OdsDataBroker();

        return useInfiniteQuery({
            queryKey: ["OdsDataGetAll", { query: query }],
            queryFn: ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetOdsDataFirstPagesAsync(query)
                }
                return broker.GetOdsDataSubsequentPagesAsync(pageParam)
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
        });
    },

    useModifyOdsData: () => {
        const broker = new OdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (odsData: OdsData) => {
                return broker.PutOdsDataAsync(odsData);
            },

            onSuccess: (data: OdsData) => {
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetById", { id: data.id }] });
            }
        });
    },

    useRemoveOdsData: () => {
        const broker = new OdsDataBroker();
        const queryClient = useQueryClient();

        return useMutation({
            mutationFn: (id: string) => {
                return broker.DeleteOdsDataByIdAsync(id);
            },
            onSuccess: (data: { id: string }) => {
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetAll"] });
                queryClient.invalidateQueries({ queryKey: ["OdsDataGetById", { id: data.id }] });
            }
        });
    },

    useGetOdsChildren: (id: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsChildren", {query: id}],
            queryFn: () => broker.GetOdsChildrenByIdAsync(id),
            staleTime: Infinity
        })

    }
}