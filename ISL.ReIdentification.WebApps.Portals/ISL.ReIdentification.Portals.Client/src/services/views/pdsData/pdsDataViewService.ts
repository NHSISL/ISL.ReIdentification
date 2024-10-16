import { useEffect, useState } from "react";
import { Guid } from "guid-typescript";
import { PdsDataView } from "../../../models/views/components/pdsData/pdsDataView";
import { pdsDataService } from "../../foundations/pdsDataAccessService";

type PdsDataViewServiceResponse = {
    mappedPdsData: PdsDataView[] | undefined;
    pages: Array<{ data: PdsDataView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: PdsDataView[] }> } | undefined;
    refetch: () => void;
};

export const pdsDataViewService = {
    useCreatePdsData: () => {
        return pdsDataService.useCreatePdsData();
    },

    useGetAllPdsData: (searchTerm?: string): PdsDataViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(UserEmail,'${searchTerm}')`;
        }

        const response = pdsDataService.useRetrieveAllPdsDataPages(query);
        const [mappedPdsData, setMappedPdsData] = useState<Array<PdsDataView>>();
        const [pages, setPages] = useState<Array<{ data: PdsDataView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const pdsDataes: Array<PdsDataView> = [];
                response.data.pages.forEach((x: { data: PdsDataView[] }) => {
                    x.data.forEach((pdsData: PdsDataView) => {
                        pdsDataes.push(new PdsDataView(
                            pdsData.rowId,
                            pdsData.pseudoNhsNumber,
                            pdsData.primaryCareProvider,
                            pdsData.primaryCareProviderBusinessEffectiveFromDate,
                            pdsData.primaryCareProviderBusinessEffectiveToDate,
                            pdsData.ccgOfRegistration,
                            pdsData.currentCcgOfRegistration,
                            pdsData.icbOfRegistration,
                            pdsData.currentIcbOfRegistration,
                        ));
                    });
                });

                setMappedPdsData(pdsDataes);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedPdsData,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetPdsDataById: (id: Guid) => {
        const query = `?$filter=id eq ${id}`;
        const response = pdsDataService.useRetrieveAllPdsDataPages(query);
        const [mappedPdsData, setMappedPdsData] = useState<PdsDataView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const pdsData = response.data.pages[0].data[0];
                const pdsDataView = new PdsDataView(
                    pdsData.rowId,
                    pdsData.pseudoNhsNumber,
                    pdsData.primaryCareProvider,
                    pdsData.primaryCareProviderBusinessEffectiveFromDate,
                    pdsData.primaryCareProviderBusinessEffectiveToDate,
                    pdsData.ccgOfRegistration,
                    pdsData.currentCcgOfRegistration,
                    pdsData.icbOfRegistration,
                    pdsData.currentIcbOfRegistration,
                );

                setMappedPdsData(pdsDataView);
            }
        }, [response.data]);

        return {
            mappedPdsData,
            ...response
        };
    },

    useUpdateLookup: () => {
        return pdsDataService.useModifyPdsData();
    },

    useRemoveLookup: () => {
        return pdsDataService.useRemovePdsData();
    },
};