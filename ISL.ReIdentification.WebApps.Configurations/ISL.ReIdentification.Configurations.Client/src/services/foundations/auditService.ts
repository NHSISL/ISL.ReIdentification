import { useInfiniteQuery, useQuery } from '@tanstack/react-query';
import AuditBroker from '../../brokers/apiBroker.audit';

export const auditService = {

    useRetrieveAllAudits: (query: string) => {
        const broker = new AuditBroker();

        return useQuery({
            queryKey: ["AccessAuditGetAll", { query: query }],
            queryFn: () => broker.GetAllAuditAsync(query),
            staleTime: Infinity
        });
    },

    useRetrieveAllAuditPages: (query: string) => {
        const auditBroker = new AuditBroker();
        return useInfiniteQuery({
            queryKey: ["AuditGetAll", { query: query }],
            queryFn: ({ pageParam  }) => {
                if (!pageParam) {
                    return auditBroker.GetAuditFirstPagesAsync(query);
                }
                return auditBroker.GetAuditSubsequentPagesAsync(pageParam);
            },
            initialPageParam: "",
            staleTime: Infinity,
            getNextPageParam: (lastPage) => lastPage.nextPage ?? null,
        });
    }
}