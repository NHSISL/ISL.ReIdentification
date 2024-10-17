import React, { FunctionComponent, useState } from "react";
import ReIdentificationDetailCard from "./reIdentificationDetailCard";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";

type ReIdentificationDetailProps = {
    children?: React.ReactNode;
};

const ReIdentificationDetail: FunctionComponent<ReIdentificationDetailProps> = (props) => {
    const {
        children
    } = props;


    let { mappedLookups: lookupsRetrieved } = lookupViewService.useGetAllLookups("");
    const postRequest = reIdentificationService.useRequestReIdentification();

    const [successfullAccessRequest, setSuccessfullAccessRequest] = useState<AccessRequestView | null>(null);


    const handleRequest = (accessRequestView: AccessRequestView) => {
        console.log(accessRequestView);

        return postRequest.mutate(accessRequestView, {
            onSuccess: (accessRequest: AccessRequestView) => {
                console.log('Re-identification successful:', accessRequest);
                setSuccessfullAccessRequest(accessRequest);
                return accessRequest;
            },
            onError: (error) => {
                console.error('Error during re-identification:', error);
                throw error;
            }
        });
    };

    return (
        <>
            {lookupsRetrieved &&
                <>
                <ReIdentificationDetailCard
                    lookups={lookupsRetrieved}
                    onReIdRequest={handleRequest}
                    successfullAccessRequest={successfullAccessRequest}/>
                    {children}
                </>
            }
        </>
    );
};

export default ReIdentificationDetail;