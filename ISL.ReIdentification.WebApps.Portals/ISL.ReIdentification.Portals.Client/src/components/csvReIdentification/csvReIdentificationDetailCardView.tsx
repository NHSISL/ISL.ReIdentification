import React, { FunctionComponent, useState } from "react";
import { Form, Button, Card, Modal, Spinner, Alert } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheck, faCopy } from "@fortawesome/free-solid-svg-icons";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { IdentificationItem } from "../../models/ReIdentifications/IdentificationItem";
import { useMsal } from "@azure/msal-react";
import UserAccessSearch from "../userAccessSearch/userAccessSearch";

interface Option {
    value: string;
    name: string;
}

interface CsvReIdentificationDetailCardViewProps {
    lookups: Array<LookupView>;
}

const CsvReIdentificationDetailCardView: FunctionComponent<CsvReIdentificationDetailCardViewProps> = (props) => {
    
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
    const [headerColumns, setHeaderColumns] = useState<string[]>([]);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [copiedToPasteBuffer, setCopiedToPasteBuffer] = useState(false);
    const clipboardAvailable = navigator.clipboard;
    const { submit, loading } = reIdentificationService.useRequestReIdentification();
    const [reIdResponse, setReIdResponse] = useState<IdentificationItem | undefined>();
    const [error, setError] = useState("");
    const account = useMsal();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (event) => {
                const text = event.target?.result as string;
                const rows = text.split("\n");
                const headers = rows[0].split(",");
                setHeaderColumns(headers);
            };
            reader.readAsText(file);
        }
    };

    const handleHeaderColumnChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedHeaderColumn(e.target.value);
    };

    const reset = () => {
        setReIdResponse(undefined);
        setSelectedLookupId("");
        setHeaderColumns([]);
        setSelectedHeaderColumn("");
        setCopiedToPasteBuffer(false);
    }

    const copyToPasteBuffer = () => {
        if (navigator.clipboard && reIdResponse) {
            navigator.clipboard.writeText(reIdResponse.identifier);
            setCopiedToPasteBuffer(true);
        }
    }

    if (!reIdResponse) {
        return (
            <>
                <Card.Subtitle className="text-start text-muted mb-3">
                    <small>
                        Please upload your csv below,
                        provide a reason why you are identifying this patient
                        and select the column that the identifier is in.
                    </small>
                </Card.Subtitle>
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="text-start">
                        <Form.Label>Upload CSV:</Form.Label>
                        <Form.Control
                            type="file"
                            name="PseudoCsv"
                            onChange={handleFileChange}
                            placeholder="Upload CSV"
                            required />
                    </Form.Group>
                    <br />
                    {headerColumns.length > 0 && (
                        <Form.Group className="text-start">
                            <Form.Label>Select Identifier Column:</Form.Label>
                            <Form.Select
                                value={selectedHeaderColumn}
                                onChange={handleHeaderColumnChange}
                                required >
                                {headerColumns.map((column, index) => (
                                    <option key={index} value={column}>
                                        {column}
                                    </option>
                                ))}
                            </Form.Select>
                        </Form.Group>
                    )}

                    <UserAccessSearch />
                    {/*<Form.Group className="text-start">*/}
                    {/*    <Form.Label>Name:</Form.Label>*/}
                    {/*    <Form.Control*/}
                    {/*        type="text"*/}
                    {/*        name="PseudoCode"*/}
                    {/*        value={pseudoCode}*/}
                    {/*        maxLength={10}*/}
                    {/*        onChange={handlePseudoCodeChange}*/}
                    {/*        placeholder="Name"*/}
                    {/*        required />*/}
                    {/*</Form.Group>*/}
                    <br />
                    <br />
                    {error && <Alert variant="danger">
                        Something went Wrong.
                    </Alert>}
                    <Button type="submit" disabled={!selectedHeaderColumn || !selectedLookupId}>
                        {!loading ? <>Send File</> : <Spinner />}
                    </Button>
                </Form>
            </>
        );
    }
    if (reIdResponse) {
        return <>
            <p className="text-start">
                NHS Number:</p>
            <Card bg="success" text="white">
                <Card.Body>
                    {reIdResponse.identifier}&nbsp;
                    {reIdResponse.hasAccess && clipboardAvailable &&
                        <FontAwesomeIcon onClick={copyToPasteBuffer} icon={copiedToPasteBuffer ? faCheck : faCopy} />
                    }
                    {!reIdResponse.hasAccess && <Modal show={true}>
                        <Modal.Header>
                            <h4>Reidentification not allowed.</h4>
                        </Modal.Header>
                        <Modal.Body>
                            <p>You have tried to reidentify a patient's that our records indicate that you do not have access to.</p>
                            <p>Check that the patient is registered to an GP practice that you have access to.</p>
                            <p>To view your ODS organisations configured in the reidentification tool click <a href="about:blank">here</a> and contact your local ICB should you need further access.</p>
                            <p>Any changes to the patient record regisistration will take 24 hours to apply to the reidentification service </p>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="primary" onClick={reset}>Start Over</Button>
                        </Modal.Footer>
                    </Modal>}
                </Card.Body>
            </Card>
            <br />

            <Button onClick={reset} variant="primary">Start Over</Button>
        </>
    }

    return <>Something went wrong.</>
}

export default CsvReIdentificationDetailCardView;