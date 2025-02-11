import React, { FunctionComponent, useState } from "react";
import { useMsal } from "@azure/msal-react";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Alert, Button, Card, Container, Form, Modal, Row, Spinner } from "react-bootstrap";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import UserAccessSearch from "../userAccessSearch/userAccessSearch";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useNavigate } from "react-router-dom";

interface Option {
    value: string;
    name: string;
}

const ImpersonationContextDetailAdd: FunctionComponent = () => {
    const [headerColumns, setHeaderColumns] = useState<string[]>([]);
    const [selectedHeaderColumn, setSelectedHeaderColumn] = useState<string>("");
    const [selectedUser, setSelectedUser] = useState<UserAccessView | undefined>();
    const { submit, loading } = reIdentificationService.useRequestReIdentificationImpersonation();
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [savedSuccessfull, setSavedSuccessfull] = useState(false);
    //const [fileName, setFileName] = useState<string>("");
    const [showHelpModal, setShowHelpModal] = useState<boolean>(false);
    const [projectName, setProjectName] = useState<string>("");

    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
    const account = useMsal();
    const navigate = useNavigate()

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const acc = account.accounts[0];
        const [firstName, lastName] = acc.name ? acc.name.split(" ") : ["", ""];

        const impersonationProjectRequest: AccessRequest = {
            csvIdentificationRequest: undefined,
            identificationRequest: undefined,
            impersonationContext: {
                id: crypto.randomUUID(),
                requesterEntraUserId: acc.idTokenClaims?.oid || "",
                requesterFirstName: firstName,
                requesterLastName: lastName,
                requesterDisplayName: acc.name || "",
                requesterEmail: acc.username,
                requesterJobTitle: " ",
                responsiblePersonEntraUserId: selectedUser?.entraUserId || "",
                responsiblePersonFirstName: selectedUser?.givenName || "",
                responsiblePersonLastName: selectedUser?.surname || "",
                responsiblePersonDisplayName: selectedUser?.displayName || "",
                responsiblePersonEmail: selectedUser?.email || "",
                responsiblePersonJobTitle: selectedUser?.jobTitle || "",
                projectName: projectName || "",
                identifierColumn: selectedHeaderColumn || "",
                isApproved: false,
                reason: selectedLookupId,
                organisation: selectedUser?.orgCode || "",
                createdBy: acc.username,
                updatedBy: acc.username,
                createdDate: new Date(),
                updatedDate: new Date(),
                purpose: ""
            }
        }

        setError("");
        submit(impersonationProjectRequest).then((message) => {
            setSuccess("Sent succesfully " + message);
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
                //setFileName(file.name);
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
    };

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
        <Container className="">
            <Row className="justify-content-md-center">
                <Card style={{ width: '50rem' }}>
                    <Card.Body>
                        <>
                            {!savedSuccessfull ? (
                                <>
                                    <Card.Title className="text-start">
                                        Add Project
                                    </Card.Title>

                                    <Card.Subtitle className="text-start text-muted mb-3">
                                        <small>
                                            Please provide the necessary details to add a new project.
                                        </small>
                                    </Card.Subtitle>
                                    <Form onSubmit={handleSubmit}>
                                        {isLoading ? <Spinner /> : <>
                                            <Form.Group className="text-start">
                                                <Form.Label><strong>Project Name:</strong></Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    value={projectName}
                                                    onChange={(e) => setProjectName(e.target.value)}
                                                    placeholder="Enter Project Name"
                                                    required
                                                />
                                            </Form.Group>
                                            <br />
                                            <UserAccessSearch selectUser={(userAccess) => { setSelectedUser(userAccess) }} labelText="Responsible Person Email Address" />

                                            <Form.Group className="text-start">
                                                <Form.Label><strong>Upload Sample CSV:</strong></Form.Label>
                                                <Button variant="link" onClick={() => setShowHelpModal(true)}>
                                                    <FontAwesomeIcon icon={faCircleInfo} className="text-primary" />
                                                </Button>

                                                <div className="d-flex align-items-center">
                                                    <Form.Control
                                                        type="file"
                                                        name="PseudoCsv"
                                                        onChange={handleFileChange}
                                                        placeholder="Upload CSV"
                                                        accept=".csv"
                                                        required
                                                    />
                                                </div>
                                                <Form.Text className="text-muted">
                                                    Please upload your CSV to choose your identfier column (other file types will be rejected). <br />
                                                    <strong>Note: the CSV will NOT be uploaded</strong>
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

                                            <Form.Group className="text-start">
                                                <Form.Label><strong>Reidentification Reason:</strong></Form.Label>
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
                                                    Please supply a reason why you are requesting.
                                                </Form.Text>
                                            </Form.Group>
                                            <br />

                                            {error && <Alert variant="danger">
                                                {error}
                                            </Alert>}
                                            {success && <Alert variant="success">
                                                {success}
                                            </Alert>}
                                            <Button type="submit" disabled={!selectedHeaderColumn || !selectedUser || !selectedHeaderColumn || !projectName || !!error}>
                                                {!loading ? <>Create New Project</> : <Spinner />}
                                            </Button>
                                        </>
                                        }
                                    </Form>
                                </>
                            ) : (
                                <>
                                    <Alert variant="success" className="mb-0">
                                        <h4>Project Approval Sent</h4>
                                        <p>The recipient should receive an email with a link for to approve this request.</p>
                                        <p>Please ensure the recipient checks their inbox and follows the instructions provided in the email to approve the file.</p>
                                            <p>Alternatively, the recipient can launch the projects page in the portal and approve from there.</p>

                                            To View your Projects Click&nbsp;
                                            <span onClick={() => navigate('/project')} style={{ color: 'blue', cursor: 'pointer', textDecoration: 'underline' }}>
                                                Here
                                            </span>
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
                    </Card.Body>
                </Card>
            </Row>
        </Container>
    );
};

export default ImpersonationContextDetailAdd;