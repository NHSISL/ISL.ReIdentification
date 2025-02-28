import { useEffect, useState } from "react";
import { auditService } from "../../foundations/auditService";
import { Audit } from "../../../models/audit/audit";

export const auditViewService = {

    useGetOdsAudit: () => {
        const query = `?$filter=AuditType eq 'ODSLoad' and startswith(auditDetail, 'Load Complete')&$orderby=createdDate desc&$top=1`;
        const response = auditService.useRetrieveAllAudits(query);
        const [mappedAudit, setMappedAudit] = useState<Array<Audit>>([]);

        useEffect(() => {
            if (response.data) {
                const audit = response.data as Audit[];
                setMappedAudit(audit);
            }
        }, [response.data]);

        return {
            mappedAudit, ...response,
            isLoading: response.isLoading
        };
    },
    useGetPdsAudit: () => {
        const query = `?$filter=AuditType eq 'PDSLoad' and startswith(auditDetail, 'Load Complete')&$orderby=createdDate desc&$top=1`;
        const response = auditService.useRetrieveAllAudits(query);
        const [mappedAudit, setMappedAudit] = useState<Array<Audit>>([]);

        useEffect(() => {
            if (response.data) {
                const audit = response.data as Audit[];
                setMappedAudit(audit);
            }
        }, [response.data]);

        return {
            mappedAudit, ...response,
            isLoading: response.isLoading
        };
    },

    useGetOdsAuditByAuditType: (auditType: string) => {
        const response = auditService.useRetrieveAllAuditsByAuditType(auditType);
        const [mappedAudit, setMappedAudit] = useState<Audit | null>(null);

        useEffect(() => {
            if (response.data) {
                const audit = response.data as Audit;
                setMappedAudit(audit);
            }
        }, [response.data]);

        return {
            mappedAudit, ...response,
            isLoading: response.isLoading
        };
    },

    useGetPdsAuditByAuditType: (auditType: string) => {
        const response = auditService.useRetrieveAllAuditsByAuditType(auditType);
        const [mappedAudit, setMappedAudit] = useState<Audit | null>(null);

        useEffect(() => {
            if (response.data) {
                const audit = response.data as Audit;
                setMappedAudit(audit);
            }
        }, [response.data]);

        return {
            mappedAudit, ...response,
            isLoading: response.isLoading
        };
    },
}