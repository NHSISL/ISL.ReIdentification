import React, { FunctionComponent, useState } from "react";
import { Card } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";
import CsvReIdentificationDetailCardView from "./csvReIdentificationDetailCardView";

interface CsvReIdentificationDetailCardProps {
    lookups: Array<LookupView>;
    onReIdRequest: (accessRequestView: AccessRequestView) => void;
    successfullAccessRequest: AccessRequestView | null;
    children?: React.ReactNode;
}

const CsvReIdentificationDetailCard: FunctionComponent<CsvReIdentificationDetailCardProps> = (props) => {
    const {
        lookups,
        onReIdRequest,
        successfullAccessRequest
    } = props;



    return (
        <Card>
            <Card.Body>
                <Card.Title>
                    CSV Re-Identification
                </Card.Title>
                <Card.Text>
                    <>
                        <CsvReIdentificationDetailCardView
                            lookups={lookups}
                            onReIdRequest={onReIdRequest}
                            successfullAccessRequest={successfullAccessRequest }
                        />
                    </>
                </Card.Text>
            </Card.Body>
        </Card>
    );
};

export default CsvReIdentificationDetailCard;