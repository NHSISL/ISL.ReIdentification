import { useEffect, useState } from "react";
import { auditService } from "../../foundations/auditService";
import { Audit } from "../../../models/audit/audit";

export const auditViewService = {

    useGetOdsAudit: () => {
        const query = `?$filter=AuditType eq 'OdsDataLoad'&$orderby=createdDate desc&$top=1`;
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
        const query = `?$filter=AuditType eq 'PdsDataLoad'&$orderby=createdDate desc&$top=1`;
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
    }
}