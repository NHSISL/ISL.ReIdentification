import { useEffect, useState } from "react";
import { csvIdentificationRequestService } from "../../foundations/csvIdentificationRequestService";
import { CsvIdentificationRequestView } from "../../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";

type CsvIdentificationRequestViewServiceResponse = {
    mappedCsvIdentificationRequests: CsvIdentificationRequestView[] | undefined;
    pages: Array<{ data: CsvIdentificationRequestView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: CsvIdentificationRequestView[] }> } | undefined;
    refetch: () => void;
};

export const csvIdentificationRequestViewService = {
    useGetAllCsvIdentificationRequestsByEntraId: (searchTerm?: string, entraId?: string): CsvIdentificationRequestViewServiceResponse => {
        let query = `?$orderby=createdDate desc`;
        query = query + `&$filter=(RequesterEntraUserId eq ${entraId} or RecipientEntraUserId eq ${entraId})`;

        if (searchTerm) {
            query = query + `&$filter=contains(RequesterFirstName,'${searchTerm}')`;
            
        }

        const response = csvIdentificationRequestService.useRetrieveAllCsvIdentificationRequestPages(query);
        const [mappedCsvIdentificationRequests, setMappedCsvIdentificationRequests] = useState<Array<CsvIdentificationRequestView>>();
        const [pages, setPages] = useState<Array<{ data: CsvIdentificationRequestView[] }>>([]);

        useEffect(() => {
            if (response.data && response.data.pages) {
                const csvIdentificationRequests: Array<CsvIdentificationRequestView> = [];
                response.data.pages.forEach((x: { data: CsvIdentificationRequestView[] }) => {
                    x.data.forEach((csvIdentificationRequest: CsvIdentificationRequestView) => {
                        csvIdentificationRequests.push(new CsvIdentificationRequestView(
                            csvIdentificationRequest.id,
                            csvIdentificationRequest.requesterEntraUserId,
                            csvIdentificationRequest.requesterFirstName,
                            csvIdentificationRequest.requesterLastName,
                            csvIdentificationRequest.requesterDisplayName,
                            csvIdentificationRequest.requesterEmail,
                            csvIdentificationRequest.recipientEntraUserId,
                            csvIdentificationRequest.recipientFirstName,
                            csvIdentificationRequest.recipientLastName,
                            csvIdentificationRequest.recipientDisplayName,
                            csvIdentificationRequest.recipientEmail,
                            csvIdentificationRequest.reason,
                            csvIdentificationRequest.organisation
                        ));
                    });
                });

                setMappedCsvIdentificationRequests(csvIdentificationRequests);
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            mappedCsvIdentificationRequests,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    }

};