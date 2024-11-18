import { useEffect, useState } from "react";
import { AccessAudit } from "../../../models/accessAudit/accessAudit";
import { accessAuditService } from "../../foundations/accessAuditService";

type AccessAuditViewServiceResponse = {
    mappedAccessAudit: AccessAudit[] | undefined;
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

        const response = accessAuditService.useRetrieveAllAccessAusitPages(query);
        const [mappedAccessAudit, setMappedAccessAudit] = useState<Array<AccessAudit>>();
        const [pages, setPages] = useState<Array<{ data: AccessAudit[] }>>([]);
        const [totalPages, setTotalPages] = useState<number>(0);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                const allData = response.data.pages.flatMap(page => page.data as AccessAudit[]);
                setMappedAccessAudit(allData);
                setPages(response.data.pages);
                const itemsPerPage = response.data.pages[0]?.data.length || 1;
                const totalItems = allData.length;
                setTotalPages(Math.ceil(totalItems / itemsPerPage));
            }
        }, [response.data]);

        return {
            mappedAccessAudit,
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