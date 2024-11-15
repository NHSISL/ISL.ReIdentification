import { useEffect, useState } from "react";
import { OdsDataView } from "../../../models/views/components/odsData/odsDataView";
import { odsDataService } from "../../foundations/odsDataAccessService";

type OdsDataViewServiceResponse = {
    mappedOdsData: OdsDataView[] | undefined;
    pages: Array<{ data: OdsDataView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: OdsDataView[] }> } | undefined;
    refetch: () => void;
};

export const odsDataViewService = {
    useGetAllOdsData: (searchTerm?: string): OdsDataViewServiceResponse => {
        let query = `?$orderby=organisationCode desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(OrganisationCode,'${searchTerm}') or contains(OrganisationName,'${searchTerm}')`;
        }

        const response = odsDataService.useRetrieveAllOdsDataPages(query);
        const [mappedOdsData, setMappedOdsData] = useState<Array<OdsDataView>>();
        const [pages, setPages] = useState<Array<{ data: OdsDataView[] }>>([]);
        
        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => page.data as OdsDataView[]);
                //const validPages = (response.data.pages as Array<{ data: OdsDataView[] }>).filter(page => page.data).flatMap(x => x.data as OdsDataView[]);
                setMappedOdsData(allData);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedOdsData,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },
};