import React, { FunctionComponent, useState } from "react";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";
import CsvReIdentificationDetailCard from "./csvReIdentificationDetailCard";
import { csvIdentificationRequestService } from "../../services/foundations/csvIdentificationRequestService";

type CsvReIdentificationDetailProps = {
    children?: React.ReactNode;
};

const CsvReIdentificationDetail: FunctionComponent<CsvReIdentificationDetailProps> = (props) => {
    const {
        children
    } = props;


    let { mappedLookups: lookupsRetrieved } = lookupViewService.useGetAllLookups("");
    const postRequest = csvIdentificationRequestService.useRequestCsvReIdentification();

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
                <CsvReIdentificationDetailCard
                    lookups={lookupsRetrieved}
                    onReIdRequest={handleRequest}
                    successfullAccessRequest={successfullAccessRequest}/>
                    {children}
                </>
            }
        </>
    );
};

export default CsvReIdentificationDetail;