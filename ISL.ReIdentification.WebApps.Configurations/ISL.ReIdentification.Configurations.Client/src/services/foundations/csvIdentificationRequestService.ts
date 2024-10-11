import { useMsal } from "@azure/msal-react";
import { Guid } from "guid-typescript";
import { useInfiniteQuery, useMutation, useQuery, useQueryClient } from "react-query";
import CsvIdentificationRequestBroker from "../../brokers/apiBroker.csvIdentificationRequest";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";


export const csvIdentificationRequestService = {
    useCreateCsvIdentificationRequest: () => {
        const broker = new CsvIdentificationRequestBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation((csvIdentificationRequest: CsvIdentificationRequest) => {
            const date = new Date();
            csvIdentificationRequest.createdDate = csvIdentificationRequest.updatedDate = date;
            csvIdentificationRequest.createdBy = csvIdentificationRequest.updatedBy = msal.accounts[0].username;

            return broker.PostCsvIdentificationRequestAsync(csvIdentificationRequest);
        },
            {
                onSuccess: (variables: CsvIdentificationRequest) => {
                    queryClient.invalidateQueries("CsvIdentificationRequestGetAll");
                    queryClient.invalidateQueries(["CsvIdentificationRequestById", { id: variables.id }]);
                }
            });
    },

    useRetrieveAllCsvIdentificationRequest: (query: string) => {
        const broker = new CsvIdentificationRequestBroker();

        return useQuery(
            ["CsvIdentificationRequestGetAll", { query: query }],
            () => broker.GetAllCsvIdentificationRequestAsync(query),
            { staleTime: Infinity });
    },

    useRetrieveAllCsvIdentificationRequestPages: (query: string) => {
        const broker = new CsvIdentificationRequestBroker();

        return useInfiniteQuery(
            ["CsvIdentificationRequestGetAll", { query: query }],
            ({ pageParam }: { pageParam?: string }) => {
                if (!pageParam) {
                    return broker.GetCsvIdentificationRequestFirstPagesAsync(query)
                }
                return broker.GetCsvIdentificationRequestSubsequentPagesAsync(pageParam)
            },
            {
                getNextPageParam: (lastPage: { nextPage?: string }) => lastPage.nextPage,
                staleTime: Infinity
            });
    },

    useModifyCsvIdentificationRequest: () => {
        const broker = new CsvIdentificationRequestBroker();
        const queryClient = useQueryClient();
        const msal = useMsal();

        return useMutation((csvIdentificationRequest: CsvIdentificationRequest) => {
            const date = new Date();
            csvIdentificationRequest.updatedDate = date;
            csvIdentificationRequest.updatedBy = msal.accounts[0].username;

            return broker.PutCsvIdentificationRequestAsync(csvIdentificationRequest);
        },
            {
                onSuccess: (data: CsvIdentificationRequest) => {
                    queryClient.invalidateQueries("CsvIdentificationRequestGetAll");
                    queryClient.invalidateQueries(["CsvIdentificationRequestGetById", { id: data.id }]);
                }
            });
    },

    useRemoveCsvIdentificationRequest: () => {
        const broker = new CsvIdentificationRequestBroker();
        const queryClient = useQueryClient();

        return useMutation((id: Guid) => {
            return broker.DeleteCsvIdentificationRequestByIdAsync(id);
        },
            {
                onSuccess: (data: { id: Guid }) => {
                    queryClient.invalidateQueries("CsvIdentificationRequestGetAll");
                    queryClient.invalidateQueries(["CsvIdentificationRequestGetById", { id: data.id }]);
                }
            });
    },
}