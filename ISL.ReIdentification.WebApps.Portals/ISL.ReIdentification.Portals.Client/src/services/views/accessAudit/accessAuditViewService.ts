import { useEffect, useState } from "react";
import { AccessAudit } from "../../../models/accessAudit/accessAudit";
import { accessAuditService } from "../../foundations/accessAuditService";

type AccessAuditViewServiceResponse = {
    mappedAccessAudit: AccessAudit[] | undefined;
    groupedAccessAudit: { [key: string]: AccessAudit[] } | undefined;
    pages: Array<{ data: AccessAudit[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    totalPages: number;
    data: { pages: Array<{ data: AccessAudit[] }> } | undefined;
    refetch: () => void;
};

export const accessAuditViewService = {
    useGetAllAccessAuditByRequestId: (searchTerm?: string, requestId?: string): AccessAuditViewServiceResponse => {
        let query = `?$filter=requestId eq ${requestId}`;

        if (searchTerm) {
            query = query + ` and (contains(pseudoIdentifier,'${searchTerm}') or 
                 contains(Email,'${searchTerm}') or 
                 contains(message,'${searchTerm}') or 
                 contains(organisation,'${searchTerm}') or 
                 contains(Reason,'${searchTerm}'))`;
        }

        query = query + `&$orderby=createdDate desc`;

        console.log("Query:", query); // Log the query to verify it's correct

        const response = accessAuditService.useRetrieveAllAccessAusitPages(query);
        const [mappedAccessAudit, setMappedAccessAudit] = useState<Array<AccessAudit>>([]);
        const [pages, setPages] = useState<Array<{ data: AccessAudit[] }>>([]);
        const [groupedAccessAudit, setGroupedAccessAudit] = useState<{ [key: string]: AccessAudit[] }>({});
        const [totalPages, setTotalPages] = useState<number>(0);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => {
                    if (Array.isArray(page.data)) {
                        return page.data;
                    } else {
                        console.error("Page data is not an array", page.data);
                        return [];
                    }
                });

                console.log("All Data:", allData);

                const groupedData = allData.reduce((acc, audit) => {
                    const requestIdKey = audit.transactionId;

                    if (!acc[requestIdKey]) {
                        acc[requestIdKey] = [];
                    }

                    acc[requestIdKey].push(audit);
                    return acc;
                }, {} as { [key: string]: AccessAudit[] });

                setGroupedAccessAudit(groupedData);
                console.log("Grouped Data:", groupedData);
                setMappedAccessAudit(allData);
                setPages(response.data.pages);
                const itemsPerPage = response.data.pages[0]?.data.length || 1;
                const totalItems = allData.length;
                setTotalPages(Math.ceil(totalItems / itemsPerPage));
            } else {
                console.error("Response data pages are not an array or are undefined", response.data);
            }
        }, [response.data]);

        return {
            mappedAccessAudit,
            groupedAccessAudit,
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            totalPages,
            data: response.data,
            refetch: response.refetch
        };
    },
};