import { useEffect } from "react";
import { auditService } from "../../foundations/auditService";

export const auditViewService = {

    useGetPdsAudit: () => {
        const query = `?$filter=auditType eq 'odsDataLoad'&$orderby=createdDate desc&$top=1`;
        const response = auditService.useRetrieveAllAudits(query);

        useEffect(() => {
            if (response.data) {
                alert("")
            }
        }, [response.data]);

        return {
            data: response.data,
        };
    },

    useGetOdsLoadAudit: () => {

    },
}