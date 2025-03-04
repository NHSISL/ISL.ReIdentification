import { useQuery } from '@tanstack/react-query';
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

    useRetrieveAllAuditsByAuditType: (auditType: string) => {
        const broker = new AuditBroker();

        return useQuery({
            queryKey: ["AccessAuditGetByAuditType", { query: auditType }],
            queryFn: () => broker.GetAuditByAuditTypeAsync(auditType),
            staleTime: Infinity
        });
    }
}