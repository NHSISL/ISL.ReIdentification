
import { useEffect, useState } from "react";
import { reIdentificationService } from "../../foundations/reIdentificationService";
import { IdentificationRequestView } from "../../../models/views/components/reIdentification/IdentificationRequestView";
import { IdentificationRequest } from "../../../models/ReIdentifications/IdentificationRequest";

type ReIdentificationViewServiceResponse = {
    mappedLookups: IdentificationRequestView[] | undefined;
    pages: Array<{ data: IdentificationRequestView[] }>;
    isLoading: boolean;
    fetchNextPage: () => void;
    isFetchingNextPage: boolean;
    hasNextPage: boolean;
    data: { pages: Array<{ data: IdentificationRequestView[] }> } | undefined;
    refetch: () => void;
};


export const reIdentificationViewService = {
    useSubmitIdsForReIdentification: (): ReIdentificationViewServiceResponse => {
        const response = reIdentificationService.useRequestReIdentification();

        const [mappedReIdentifications, setMappedReIdentifications] =
            useState<Array<IdentificationRequestView> | undefined>(undefined);

        useEffect(() => {
            if (Array.isArray(response.data)) {
                const reIdentifications = response.data.map((identificationRequest: IdentificationRequest) =>
                    new IdentificationRequestView(
                        identificationRequest.id,
                        identificationRequest.identificationItems,
                        identificationRequest.entraUserId,
                        identificationRequest.givenName,
                        identificationRequest.surname,
                        identificationRequest.displayName,
                        identificationRequest.jobTitle,
                        identificationRequest.email,
                        identificationRequest.purpose,
                        identificationRequest.organisation,
                        identificationRequest.reason,
                    )
                );

                setMappedReIdentifications(reIdentifications);
            } else {
                setMappedReIdentifications(undefined); // Clear state if no valid array
            }
        }, [response.data]);

        return {
            mappedReIdentifications,
            ...response,
        };
    }
};