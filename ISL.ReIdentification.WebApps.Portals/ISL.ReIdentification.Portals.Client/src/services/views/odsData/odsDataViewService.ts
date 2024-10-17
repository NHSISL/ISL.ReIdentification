import { useEffect, useState } from "react";
import { Guid } from "guid-typescript";
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
    useCreateOdsData: () => {
        return odsDataService.useCreateOdsData();
    },

    useGetAllOdsData: (searchTerm?: string): OdsDataViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;

        if (searchTerm) {
            query = query + `&$filter=contains(UserEmail,'${searchTerm}')`;
        }

        const response = odsDataService.useRetrieveAllOdsDataPages(query);
        const [mappedOdsData, setMappedOdsData] = useState<Array<OdsDataView>>();
        const [pages, setPages] = useState<Array<{ data: OdsDataView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const odsDataes: Array<OdsDataView> = [];
                response.data.pages.forEach((x: { data: OdsDataView[] }) => {
                    x.data.forEach((odsData: OdsDataView) => {
                        odsDataes.push(new OdsDataView(
                            odsData.id,
                            odsData.organisationCode_Root,
                            odsData.organisationPrimaryRole_Root,
                            odsData.organisationCode_Parent,
                            odsData.organisationPrimaryRole_Parent,
                            odsData.relationshipStartDate,
                            odsData.relationshipEndDate,
                            odsData.path,
                            odsData.depth,
                            odsData.pdsData
                        ));
                    });
                });

                setMappedOdsData(odsDataes);
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

    useGetOdsDataById: (id: Guid) => {
        const query = `?$filter=id eq ${id}`;
        const response = odsDataService.useRetrieveAllOdsDataPages(query);
        const [mappedOdsData, setMappedOdsData] = useState<OdsDataView>();

        useEffect(() => {
            if (response.data && response.data.pages && response.data.pages[0].data[0]) {
                const odsData = response.data.pages[0].data[0];
                const odsDataView = new OdsDataView(
                    odsData.id,
                    odsData.organisationCode_Root,
                    odsData.organisationPrimaryRole_Root,
                    odsData.organisationCode_Parent,
                    odsData.organisationPrimaryRole_Parent,
                    odsData.relationshipStartDate,
                    odsData.relationshipEndDate,
                    odsData.path,
                    odsData.depth,
                    odsData.pdsData
                );

                setMappedOdsData(odsDataView);
            }
        }, [response.data]);

        return {
            mappedOdsData,
            ...response
        };
    },

    useUpdateLookup: () => {
        return odsDataService.useModifyOdsData();
    },

    useRemoveLookup: () => {
        return odsDataService.useRemoveOdsData();
    },
};