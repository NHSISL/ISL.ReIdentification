import React, { FunctionComponent, useState } from "react";
import { Card } from "react-bootstrap";
import ReIdentificationDetailCardView from "./reIdentificationDetailCardView";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";

interface ReIdentificationDetailCardProps {
    lookups: Array<LookupView>;
    onReIdRequest: (accessRequestView: AccessRequestView) => void;
    successfullAccessRequest: AccessRequestView | null;
    children?: React.ReactNode;
}

const ReIdentificationDetailCard: FunctionComponent<ReIdentificationDetailCardProps> = (props) => {
    const {
        lookups,
        onReIdRequest,
        successfullAccessRequest
    } = props;



    return (
        <Card>
            <Card.Body>
                <Card.Title>
                    Re-Identification
                </Card.Title>
                <Card.Text>
                    <>
                        <ReIdentificationDetailCardView
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

export default ReIdentificationDetailCard;