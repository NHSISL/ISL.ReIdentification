import React, { FunctionComponent, useState } from "react";
import { Container, Row, Col, Form, Button, Alert } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { IdentificationRequestView } from "../../models/views/components/reIdentification/IdentificationRequestView";
import { IdentificationItemView } from "../../models/views/components/reIdentification/IdentificationItemView";
import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";

interface Option {
    value: string;
    name: string;
}

interface ReIdentificationDetailCardViewProps {
    lookups: Array<LookupView>;
    onReIdRequest: (accessRequestView: AccessRequestView) => void;
    successfullAccessRequest: AccessRequestView | null;
}

const ReIdentificationDetailCardView: FunctionComponent<ReIdentificationDetailCardViewProps> = (props) => {
    const { lookups, onReIdRequest, successfullAccessRequest } = props;

    const [pseudoCode, setPseudoCode] = useState<string>("");
    const [purpose, setPurpose] = useState<string>("");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
    const { accounts } = useMsal();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const identificationRequest = new IdentificationRequestView();

        console.log(accounts[0]);

        identificationRequest.identificationItems = [
            new IdentificationItemView("1", pseudoCode, "", false, false)
        ];

        identificationRequest.id = crypto.randomUUID();
        identificationRequest.identificationItems[0].identifier = pseudoCode;
        identificationRequest.entraUserId = accounts[0].idTokenClaims?.oid || "";
        identificationRequest.email = accounts[0].username || "";
        identificationRequest.purpose = purpose;
        identificationRequest.organisation = "Nel ICB";
        identificationRequest.reason = selectedLookupId;

        const accessRequest = new AccessRequest({
            identificationRequest: identificationRequest
        });

        onReIdRequest(accessRequest);
    };

    const handlePseudoCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPseudoCode(e.target.value);
    };

    const handlePurposeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPurpose(e.target.value);
    };

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const lookupOptions: Array<Option> = [
        { value: "", name: "Please select..." },
        ...lookups.map((lookup) => ({
            value: lookup.value.toString() || "0",
            name: lookup.name || "",
        })),
    ];

    return (
        <Container fluid>
            <Row className="justify-content-md-center">
                <Col md={12}>
                    <Form onSubmit={handleSubmit}>
                        <Form.Group>
                            <Form.Label>Pseudo ID:</Form.Label>
                            <Form.Control
                                type="text"
                                name="PseudoCode"
                                value={pseudoCode}
                                maxLength={10}
                                onChange={handlePseudoCodeChange}
                                required
                            />

                            <Form.Label>Lookup:</Form.Label>
                            <Form.Select
                                value={selectedLookupId}
                                onChange={handleLookupChange}
                                required >
                                {lookupOptions.map((option) => (
                                    <option key={option.value} value={option.value}>
                                        {option.name}
                                    </option>
                                ))}
                            </Form.Select>

                            <Form.Label>Purpose of Request:</Form.Label>
                            <Form.Control as="textarea"
                                rows={3}
                                type="text"
                                name="Purpose"
                                value={purpose}
                                onChange={handlePurposeChange}
                                maxLength={255}
                                required
                            />
                        </Form.Group>

                        <br />
                        <Button type="submit">Get NHS Number</Button>
                    </Form>
                    <br />


                    {successfullAccessRequest !== null && (
                        <>
                            {successfullAccessRequest.identificationRequest ? (
                                successfullAccessRequest.identificationRequest.identificationItems.map((item, index) => (
                                    item.hasAccess ? (
                                        <div key={index}>
                                            TODO: This will need a new Route and Page to display nhs number without the form above.
                                        </div>
                                    ) : (
                                        <Alert key={index} variant="danger">
                                            <Alert.Heading>Identification Request</Alert.Heading>
                                            <span>Request ID: <strong>{successfullAccessRequest.identificationRequest?.email || "no email found."}</strong></span><br />
                                            <span>Is ReIdentified: <strong>{item.isReidentified ? "Yes" : "No"}</strong></span><br />
                                            <span>Has Access: <strong>{item.hasAccess ? "Yes" : "No"}</strong></span> <br />
                                            <span>Message: <strong>{item.message}</strong></span><br />

                                            <span>Reason: <strong>{successfullAccessRequest.identificationRequest?.reason}</strong></span><br />
                                            <span>Purpose: <strong>{successfullAccessRequest.identificationRequest?.purpose}</strong></span><br />
                                        </Alert>
                                    )
                                ))
                            ) : (
                                <p>No Identification Request Data Available</p>
                            )}
                        </>
                    )}
                </Col>
            </Row>
        </Container>
    );
}

export default ReIdentificationDetailCardView;
