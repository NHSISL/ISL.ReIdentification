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
    const [csvData, setCsvData] = useState<string | null>(null);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [selectedUser, setSelectedUser] = useState<UserAccessView | undefined>();
    const { submit, loading } = reIdentificationService.useRequestReIdentificationCsv();
    const [error, setError] = useState("");
    const [savedSuccessfull, setSavedSuccessfull] = useState(false);
    const [fileName, setFileName] = useState<string>("");
    const account = useMsal();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const acc = account.accounts[0];
        const [firstName, lastName] = acc.name ? acc.name.split(" ") : ["", ""];

        const csvIdentificationRequest: AccessRequest = {
            impersonationContext: undefined,
            identificationRequest: undefined,
            csvIdentificationRequest: {
                id: crypto.randomUUID(),
                requesterEntraUserId: acc.idTokenClaims?.oid,
                requesterFirstName: firstName,
                requesterLastName: lastName,
                requesterDisplayName: acc.name,
                requesterEmail: acc.username,
                requesterJobTitle: "TODO: Job Title",
                recipientEntraUserId: selectedUser?.entraUserId || "",
                recipientFirstName: selectedUser?.givenName || "",
                recipientLastName: selectedUser?.surname || "",
                recipientDisplayName: selectedUser?.displayName || "",
                recipientEmail: selectedUser?.email || "",
                recipientJobTitle: selectedUser?.jobTitle || "",
                data: csvData || "",
                identifierColumn: selectedHeaderColumn,
                organisation: selectedUser?.orgCode || "",
                createdBy: acc.username,
                updatedBy: acc.username,
                createdDate: new Date(),
                updatedDate: new Date(),
                filepath: fileName.replace(/\.csv$/i, "") 
            }
        }
        setError("");
        submit(csvIdentificationRequest).then((d) => {
            console.log("Sent", d);
            toastSuccess("CSV Sent");
            setSavedSuccessfull(true)
        }).catch(() => {
            setSavedSuccessfull(false)
            setError("Something went wrong when saving, please contact an administrator");
        })
    };


    //Check for blank rows
    //

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setError("");
        const file = e.target.files?.[0];
        if (file) {
            const sizeInBytes = file.size;
            const sizeInMB = (sizeInBytes / (1024 * 1024)).toFixed(2);
            console.log(`File size: ${sizeInBytes} bytes (${sizeInMB} MB)`);

            if (sizeInBytes > 1024 * 1024) {
                setError("File size exceeds 1MB. Please upload a smaller file.");
                return;
            }

            if (file.name.endsWith(".csv")) {
                setFileName(file.name);
                const reader = new FileReader();
                reader.onload = (event) => {
                    const text = event.target?.result as string;
                    const rows = text.split("\n");
                    const headers = rows[0].split(",");

                    if (headers.length <= 1) {
                        setError("The CSV file does not contain a valid header row.");
                        return;
                    }

                    setHeaderColumns(headers);
                    const uint8Array = new TextEncoder().encode(text);
                    const base64String = btoa(String.fromCharCode(...uint8Array));
                    setCsvData(base64String);
                };
                reader.readAsText(file);
            } else {
                setError("Please upload a valid .csv file.");
            }
        } else {
            setError("No file selected. Please upload a .csv file.");
        }
    };

    const handleHeaderColumnChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedHeaderColumn(e.target.value);
    };

    const renderTooltip = (props: OverlayTriggerProps) => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to upload a CSV of pseudo identifiers for reidentification.
        </Tooltip>
    );

    return (
        <>
            {!savedSuccessfull ? (
                <>
                    <Card.Title className="text-start">
                        <OverlayTrigger placement="right" overlay={renderTooltip}>
                            <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                        </OverlayTrigger>&nbsp;Dataset Upload
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
                                accept=".csv"
                                required />
                        </Form.Group>
                        <br />
                        {headerColumns.length > 0 && (
                            <Form.Group className="text-start">
                                <Form.Label>Select Identifier Column:</Form.Label>
                                <Form.Select
                                    value={selectedHeaderColumn}
                                    onChange={handleHeaderColumnChange}
                                    required>
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
                            {error}
                        </Alert>}
                        <Button type="submit" disabled={!selectedHeaderColumn || !selectedUser || !selectedHeaderColumn}>
                            {!loading ? <>Send File</> : <Spinner />}
                        </Button>
                    </Form>
                </>
            ) : (
                <>
                    SENT - Need Some Copy here
                </>
            )}
        </>
    );

    return <>{error ? error : "Something went wrong."}</>;
}

export default CsvReIdentificationDetailCardView;