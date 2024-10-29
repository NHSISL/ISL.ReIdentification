import React, { FunctionComponent, useState } from "react";
import { Form, Button, Card, Spinner, Alert, OverlayTrigger, Tooltip, OverlayTriggerProps } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import UserAccessSearch from "../userAccessSearch/userAccessSearch";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { toastSuccess } from "../../brokers/toastBroker.success";

const CsvReIdentificationDetailCardView: FunctionComponent = () => {

    const [headerColumns, setHeaderColumns] = useState<string[]>([]);
    const [csvData, setCsvData] = useState<Uint8Array | null>(null);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [selectedUser, setSelectedUser] = useState<UserAccessView | undefined>();
    const { submit, loading } = reIdentificationService.useRequestReIdentification();
    const [error, setError] = useState("");
    const account = useMsal();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const acc = account.accounts[0];

        const csvIdentificationRequest: AccessRequest = {
            impersonationContext: undefined,
            identificationRequest: undefined,
            csvIdentificationRequest: {
                id: crypto.randomUUID(),
                requesterEntraUserId: acc.idTokenClaims?.oid,
                requesterFirstName: "FIRSTNAME",
                requesterLastName: "LASTNAME",
                requesterDisplayName: "LASTNAME",
                requesterEmail: acc.username,
                requesterJobTitle: "TODO: Job Title",
                recipientEntraUserId: selectedUser?.entraUserId || "",
                recipientFirstName: selectedUser?.givenName || "",
                recipientLastName: selectedUser?.surname || "",
                recipientDisplayName: selectedUser?.displayName || "",
                recipientEmail: selectedUser?.email || "",
                recipientJobTitle: selectedUser?.jobTitle || "",
                data: csvData || new Uint8Array(),
                identifierColumn: selectedHeaderColumn,
            }
        }
        setError("");
        submit(csvIdentificationRequest).then((d) => {
            console.log("Sent", d);
            toastSuccess("CSV Sent");
        }).catch(() => {
            setError("Something went wrong");
        })
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
                setCsvData(new TextEncoder().encode(text));
            };
            reader.readAsText(file);
        }
    };

    const handleHeaderColumnChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedHeaderColumn(e.target.value);
    };

    const renderTooltip = (props: OverlayTriggerProps) => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to upload a CSV of pseudo identifiers for reidentification, please also select the column used for the pseudo identifier.
        </Tooltip>
    );

   
        return (
            <>
                <Card.Title className="text-start">
                    <OverlayTrigger placement="right" overlay={renderTooltip}>
                        <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                    </OverlayTrigger>&nbsp;CSV Upload
                </Card.Title>

                <Card.Subtitle className="text-start text-muted mb-3">
                    <small>
                        Please upload your csv below,
                        provide a reason why you are identifying this patient
                        and select the column that the identifier is in.
                    </small>
                </Card.Subtitle>
                <Form onSubmit={handleSubmit}>
                    <UserAccessSearch selectUser={(userAccess) => { setSelectedUser(userAccess) }} />

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
                    <br />
                    {error && <Alert variant="danger">
                        Something went Wrong.
                    </Alert>}
                    <Button type="submit" disabled={!selectedHeaderColumn || !selectedUser || !selectedHeaderColumn}>
                        {!loading ? <>Send File</> : <Spinner />}
                    </Button>
                </Form>
            </>
        );

    return <>Something went wrong.</>
}

export default CsvReIdentificationDetailCardView;