
import { useEffect, useState } from "react";
import { reIdentificationService } from "../../foundations/reIdentificationService";
import { IdentificationRequestView } from "../../../models/views/components/reIdentification/IdentificationRequestView";
import { IdentificationRequest } from "../../../models/ReIdentifications/IdentificationRequest";

type ReIdentificationViewServiceResponse = {
    mappedReIdentifications: IdentificationRequestView[] | undefined;
};


export const reIdentificationViewService = {
    useSubmitIdsForReIdentification: (): ReIdentificationViewServiceResponse => {
        const response = reIdentificationService.useRequestReIdentification();

        const [mappedReIdentifications, setMappedReIdentifications] =
            useState<Array<IdentificationRequestView> | undefined>();

        useEffect(() => {
            if (Array.isArray(response.data)) {
                const reIdentifications = response.data as IdentificationRequest[]; 
                setMappedReIdentifications(reIdentifications);
            } else {
                setMappedReIdentifications([]); // Clear state if no valid array
            }
        }, [response.data]);

        return {
            mappedReIdentifications,
            ...response,
        };
    }
};