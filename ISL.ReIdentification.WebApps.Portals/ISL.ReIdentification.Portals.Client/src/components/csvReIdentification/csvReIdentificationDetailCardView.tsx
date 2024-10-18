import React, { FunctionComponent, useState, useEffect } from "react";
import { Container, Row, Col, Form, Button, Alert } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";
import { CsvIdentificationRequestView } from "../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";
import { useMsal } from "@azure/msal-react";

interface Option {
    value: string;
    name: string;
}

interface CsvReIdentificationDetailCardViewProps {
    lookups: Array<LookupView>;
    onReIdRequest: (accessRequestView: AccessRequestView) => void;
    successfullAccessRequest: AccessRequestView | null;
}

const CsvReIdentificationDetailCardView: FunctionComponent<CsvReIdentificationDetailCardViewProps> = (props) => {
    const { lookups, onReIdRequest, successfullAccessRequest } = props;
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
    const [uploadedFile, setUploadedFile] = useState<Uint8Array | null>(null);
    const [recipientEmail, setRecipientEmail] = useState<string>("");
    const [identifierColumn, setIdentifierColumn] = useState<string>("");
    const { accounts } = useMsal();

    useEffect(() => {
        // Populate the recipient email when the component loads
        if (accounts && accounts.length > 0) {
            setRecipientEmail(accounts[0].username || "");
        }
    }, [accounts]);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!uploadedFile) {
            alert("Please upload a CSV file.");
            return;
        }

        const csvIdentificationRequest = new CsvIdentificationRequestView();

        csvIdentificationRequest.id = crypto.randomUUID();
        csvIdentificationRequest.requesterEmail = recipientEmail;
        csvIdentificationRequest.organisation = "Nel ICB";
        csvIdentificationRequest.reason = selectedLookupId;
        csvIdentificationRequest.identifierColumn = identifierColumn;
        csvIdentificationRequest.data = uploadedFile;

        const accessRequest = new AccessRequest({
            csvIdentificationRequest: csvIdentificationRequest
        });

        onReIdRequest(accessRequest);
    };

    const handleRecipientChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setRecipientEmail(e.target.value);
    };

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files ? e.target.files[0] : null;

        if (file) {
            const reader = new FileReader();
            reader.onload = (event) => {
                if (event.target?.result) {
                    const base64String = btoa(String.fromCharCode(...new Uint8Array(event.target.result as ArrayBuffer)));
                    setUploadedFile(base64String);
                }
            };
            reader.readAsArrayBuffer(file);
        } else {
            setUploadedFile(null);
        }
    };

    const handleIdentifierColumnChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIdentifierColumn(e.target.value);
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

                        <Form.Group controlId="formFile" className="mb-3">
                            <Form.Label>CSV Upload</Form.Label>
                            <Form.Control
                                type="file"
                                accept=".csv"
                                onChange={handleFileChange}
                                required
                            />
                        </Form.Group>

                        <Form.Group>
                            <Form.Label>Identifier Column <small>(please type the name of the colomn that has the psuedo value in.)</small>:</Form.Label>
                            <Form.Control
                                type="text"
                                name="identifierColumn"
                                value={identifierColumn}
                                onChange={handleIdentifierColumnChange}
                                placeholder="Enter the column name that contains the identifier"
                                required
                            />
                        </Form.Group>

                        <Form.Group>
                            <Form.Label>Recipient:</Form.Label>
                            <Form.Control
                                type="text"
                                name="Recipient"
                                value={recipientEmail}
                                onChange={handleRecipientChange}
                                required
                            />
                        </Form.Group>

                        <Form.Group>
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
                        </Form.Group>

                        

                        <br />
                        <Button type="submit">Upload</Button>
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

export default CsvReIdentificationDetailCardView;
