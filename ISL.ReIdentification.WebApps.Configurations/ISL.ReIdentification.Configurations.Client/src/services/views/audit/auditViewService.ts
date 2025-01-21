import { useEffect, useState } from "react";
import { Audit } from "../../../models/audit/audit";
import { auditService } from "../../foundations/auditService";

type AuditViewServiceResponse = {
    pages: Array<{ data: Audit[] }> | undefined;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: Audit[] }> } | undefined;
    refetch: () => void;
};

export const auditViewService = {
    useGetAllAudit: (searchTerm?: string): AuditViewServiceResponse => {
        let query = `?$`;

        if (searchTerm) {
            query = query + ` and (contains(AuditType,'${searchTerm}'))`;
        }

        query = query + `&$orderby=createdDate desc`;

        const response = auditService.useRetrieveAllAuditPages(query);
        const [pages, setPages] = useState<Array<{ data: Audit[] }>>([]);

        useEffect(() => {
            if (response.data && Array.isArray(response.data.pages)) {
                setPages(response.data.pages);
            }
        }, [response.data]);

        return {
            pages,
            isLoading: response.isLoading,
            fetchNextPage: response.fetchNextPage,
            isFetchingNextPage: response.isFetchingNextPage,
            hasNextPage: !!response.hasNextPage,
            data: response.data,
            refetch: response.refetch
        };
    },

    useGetOdsLoadAudit: () => {

    },
}