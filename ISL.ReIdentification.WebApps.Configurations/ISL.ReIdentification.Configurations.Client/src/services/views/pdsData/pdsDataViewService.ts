import { useEffect, useState } from "react";
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
        let query = `?$orderby=orgCode desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(pseudoNhsNumber,'${searchTerm}') or contains(orgCode,'${searchTerm}') or contains(organisationName,'${searchTerm}')`;
        }

        const response = pdsDataService.useRetrieveAllPdsDataPages(query);
        const [mappedPdsData, setMappedPdsData] = useState<Array<PdsDataView>>();
        const [pages] = useState<Array<{ data: PdsDataView[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const validPages = (response.data.pages as Array<{ data: PdsDataView[] }>).filter(page => page.data).flatMap(x => x.data as PdsDataView[]);
                setMappedPdsData(validPages);
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

    useGetPdsDataById: (id: string) => {
        const query = `?$filter=id eq ${id}`;
        const response = pdsDataService.useRetrieveAllPdsDataPages(query);
        const [mappedPdsData, setMappedPdsData] = useState<PdsDataView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const pdsData = response.data.pages[0].data[0];
                const pdsDataView = new PdsDataView(
                    pdsData.id,
                    pdsData.pseudoNhsNumber,
                    pdsData.OrgCode,
                    pdsData.OrganisationName,
                    pdsData.RelationshipWithOrganisationEffectiveFromDate,
                    pdsData.RelationshipWithOrganisationEffectiveToDate
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