import React, { FunctionComponent, useState } from "react";
import { Form, Button, Card, Spinner, Alert, OverlayTrigger, Tooltip, Modal } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import UserAccessSearch from "../userAccessSearch/userAccessSearch";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { OverlayInjectedProps } from "react-bootstrap/esm/Overlay";

interface Option {
    value: string;
    name: string;
}

const CsvReIdentificationDetailCardView: FunctionComponent = () => {

    const [headerColumns, setHeaderColumns] = useState<string[]>([]);
    const [csvData, setCsvData] = useState<string | null>(null);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [selectedHeaderColumnIndex, setSelectedHeaderColumnIndex] = useState<number>(0);
    const [selectedUser, setSelectedUser] = useState<UserAccessView | undefined>();
    const { submit, loading } = reIdentificationService.useRequestReIdentificationCsv();
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [savedSuccessfull, setSavedSuccessfull] = useState(false);
    const [fileName, setFileName] = useState<string>("");
    const [hasHeaderRecord, setHasHeaderRecord] = useState<boolean>(false);
    const [showHelpModal, setShowHelpModal] = useState<boolean>(false);

    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
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
                requesterJobTitle: " ",
                recipientEntraUserId: selectedUser?.entraUserId,
                recipientFirstName: selectedUser?.givenName || "",
                recipientLastName: selectedUser?.surname || "",
                recipientDisplayName: selectedUser?.displayName || "",
                recipientEmail: selectedUser?.email || "",
                recipientJobTitle: selectedUser?.jobTitle || "",
                data: csvData || "",
                reason: selectedLookupId,
                organisation: selectedUser?.orgCode,
                createdBy: acc.username,
                updatedBy: acc.username,
                createdDate: new Date(),
                updatedDate: new Date(),
                filepath: fileName.replace(/\.csv$/i, ""),
                hasHeaderRecord: hasHeaderRecord,
                identifierColumnIndex: selectedHeaderColumnIndex
            }
        }
        setError("");
        submit(csvIdentificationRequest).then((d) => {
            console.log("Sent", d);
            setSavedSuccessfull(true)
        }).catch(() => {
            setSavedSuccessfull(false)
            setError("Something went wrong when saving, please contact an administrator");
        })
    };

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
                    let text = event.target?.result as string;
                    text = text.replace(/\r/g, "");
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
        const selectedColumn = e.target.value;
        setSelectedHeaderColumn(selectedColumn);
        const index = headerColumns.indexOf(selectedColumn);
        setSelectedHeaderColumnIndex(index);

        if (csvData) {
            const decodedData = atob(csvData);
            const rows = decodedData.split("\n");

            if (rows.length > 1) {
                const nextRow = rows[1].split(",");
                const value = nextRow[index];

                if (/^\d{10}$/.test(value)) {
                    setError("");
                    setSuccess(`The value "${value}" in the next row at the selected column index is 10 digits long and is a valid Pseudo Identifier.`);
                } else {
                    setSuccess("");
                    setError(`The value "${value}" in the next row for the selected column index is not a valid Pseudo Identifier, please follow the guidance in the help section.`);
                }
            }
        }
    };
    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setHasHeaderRecord(e.target.checked);
    };

    const renderTooltip = (props: OverlayInjectedProps): React.ReactElement => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to upload a CSV of pseudo identifiers for reidentification.
        </Tooltip>
    );

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const lookupOptions: Array<Option> = [
        { value: "", name: "Select Reason..." },
        ...mappedLookups.map((lookup) => ({
            value: lookup.value.toString() || "0",
            name: lookup.value || "",
        })),
    ];

    return (
        <>
            {!savedSuccessfull ? (
                <>



                    <Card.Header>
                        <Card.Title className="text-start">
                            <OverlayTrigger placement="right" overlay={renderTooltip}>
                                <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                            </OverlayTrigger>&nbsp;Dataset Upload
                        </Card.Title>
                    </Card.Header>

                    <Card.Body>

                        <Card.Subtitle className="text-start text-muted mb-3">
                            <small>
                                Please upload your CSV file below, specify the reason for identifying these patients
                                from the drop-down menu, and select the column containing the identifier.
                            </small>
                        </Card.Subtitle>
                        <Form onSubmit={handleSubmit}>
                            <UserAccessSearch selectUser={(userAccess) => { setSelectedUser(userAccess) }} labelText="Recipient Email Address" />
                            <Form.Group className="text-start">
                                <Form.Label><strong>Upload CSV:</strong></Form.Label>
                                <Button variant="link" onClick={() => setShowHelpModal(true)}>
                                    <FontAwesomeIcon icon={faCircleInfo} className="text-primary" />
                                </Button>

                                <Form.Check
                                    type="checkbox"
                                    label="Has Header Record"
                                    id="hasHeadRecord"
                                    onChange={handleCheckboxChange} />

                                <div className="d-flex align-items-center">
                                    <Form.Control
                                        type="file"
                                        name="PseudoCsv"
                                        onChange={handleFileChange}
                                        placeholder="Upload CSV"
                                        accept=".csv"
                                        required />

                                </div>
                                <Form.Text className="text-muted">
                                    Please upload your CSV (other file types will be rejected).
                                </Form.Text>
                            </Form.Group>
                            <br />
                            {headerColumns.length > 0 && (
                                <>
                                    <Form.Group className="text-start">
                                        <Form.Label><strong>Select Identifier Column From Csv:</strong></Form.Label>
                                        <Form.Select
                                            value={selectedHeaderColumn}
                                            onChange={handleHeaderColumnChange}
                                            required>
                                            <option value="" disabled>Please select a column...</option>
                                            {headerColumns.map((column, index) => (
                                                <option key={index} value={column}>
                                                    {`Col-${index + 1} - ${column}`}
                                                </option>
                                            ))}
                                        </Form.Select>
                                        <Form.Text className="text-muted">
                                            Please choose the correct column for the Pseudo Identifier.
                                        </Form.Text>
                                    </Form.Group>
                                    <br />
                                </>
                            )}
                            {isLoading ? <Spinner /> : <>
                                <Form.Group className="text-start">
                                    <Form.Label><strong>Re-identification Reason:</strong></Form.Label>
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
                                    <Form.Text className="text-muted">
                                        Please supply a reason why you are requesting to Reidentify.
                                    </Form.Text>
                                </Form.Group>
                            </>}
                            <br />

                            {error && <Alert variant="danger">
                                {error}
                            </Alert>}
                            {success && <Alert variant="success">
                                {success}
                            </Alert>}
                            <Button type="submit" disabled={!selectedHeaderColumn || !selectedUser || !selectedHeaderColumn || !!error}>
                                {!loading ? <>Send File</> : <Spinner />}
                            </Button>
                        </Form>
                    </Card.Body>
                </>
            ) : (
                <>
                    <Alert variant="success" className="mb-0">
                        <h4>CSV Sent</h4>
                        <p>The recipient should receive an email with a link to download the re-identified file.</p>
                        <p>Please ensure the recipient checks their inbox and follows the instructions provided in the email to access the file.</p>
                        <p>Alternatively, the recipient can launch their worklist in the portal and re-identify from there.</p>
                        <p><strong>Note:</strong> Files will last for 7 days and then be removed from the system.</p>
                    </Alert>

                </>
            )}

            <Modal show={showHelpModal} onHide={() => setShowHelpModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title><FontAwesomeIcon icon={faCircleInfo} className="text-primary" /> CSV Help</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>
                        The identifier column needs to be <i>10</i> digits.
                        You can easily get your data in this format by applying a format in EXCEL and then saving the CSV file again if its not.
                    </p>
                    <p>
                        <strong>Method 1: Using a Custom Number Format</strong>
                        <ol>
                            <li>Select the column that contains the numbers.</li>
                            <li>Right-click on the selected column and choose Format Cells.</li>
                            <li>In the Format Cells dialog box, go to the Number tab and choose Custom from the list on the left.</li>
                            <li>
                                In the Type field, enter the following format code:This will ensure that all numbers in the column
                                are displayed with 10 digits, padding with leading zeroes if necessary. Click OK.
                            </li>
                        </ol>
                    </p>
                    This method works well if the numbers are stored as numerical values but will display as <i>10</i> digits.
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowHelpModal(false)}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );

    return <>{error ? error : "Something went wrong."}</>;
}

export default CsvReIdentificationDetailCardView;