import { faCheck, faCircleInfo, faCopy, faLeftLong, faRightLong, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react";
import { Button, Card, CardBody, Col, Modal, Row, Table, Toast, ToastContainer } from "react-bootstrap";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { ReIdRecord } from "../../types/ReIdRecord";
import CopyIcon from "../core/copyIcon";

type ReportToastProps = {
    position: ToastPosition;
    hidden: boolean;
    hide: (hide: boolean) => void;
    clearList: () => void;
    reidentifications: ReIdRecord[];
    lastSelectedPseudo?: string;
}

const ReportToast: FunctionComponent<ReportToastProps> = (props) => {
    const { position, hidden, hide, clearList, reidentifications } = props;
    const [copiedToPasteBuffer, setCopiedToPasteBuffer] = useState(false);
    const [showNoAccessInfo, setShowNoAccessInfo] = useState(false);
    const [pageNumber, setPageNumber] = useState(0);
    const [showHistory, setShowHistory] = useState(false);
    const itemsPerPage = 10;
    const clipboardAvailable = navigator.clipboard;

    const copyToPasteBuffer = (identifer?: string) => {
        if (navigator.clipboard && identifer) {
            navigator.clipboard.writeText(identifer);
            setCopiedToPasteBuffer(true);
            setTimeout(() => setCopiedToPasteBuffer(false), 1000);
        }
    }

    return <ToastContainer position={position || "bottom-end"} hidden={hidden || reidentifications.length === 0} style={{ marginTop: "33px" }}>
        <Toast onClose={() => hide(true)}>
            <Toast.Header>
                <strong className="me-auto">Reidentification</strong>
                <Button size="sm" onClick={clearList}>Clear History</Button>
            </Toast.Header>
            <Toast.Body>
                {reidentifications.length > 0 && <>
                    {reidentifications[0].hasAccess ? <Card bg="success" text="white">
                        <Card.Body>
                            Last Selected Record:&nbsp;
                            {reidentifications[0].nhsnumber}&nbsp;
                            {clipboardAvailable &&
                                <FontAwesomeIcon onClick={() => copyToPasteBuffer(reidentifications[0].nhsnumber)} icon={copiedToPasteBuffer ? faCheck : faCopy} />
                            }
                        </Card.Body>
                    </Card>
                        :
                        <Card bg="danger" text="white">
                            <CardBody>
                                You do not have permissions to reidentify this patient.
                            </CardBody>
                        </Card>
                    }
                </>}

                {showHistory &&
                    <> <br />
                        <Table size="sm" bordered>
                            <tbody>
                                {reidentifications.slice(pageNumber * itemsPerPage, (pageNumber * itemsPerPage) + itemsPerPage).map((ri) => <tr key={ri.identifer}>
                                    <td>{ri.isHx ? ri.pseudo : "---"}</td>
                                    {ri.loading ? <td>
                                        <FontAwesomeIcon icon={faSpinner} pulse />
                                    </td> : <>
                                        {ri.hasAccess ? <>
                                            <td>{ri.nhsnumber}</td><td><CopyIcon content={ri.nhsnumber || ""} resetTime={1000} />
                                            </td>
                                        </>
                                            : <td colSpan={2}>
                                                NO ACCESS <FontAwesomeIcon icon={faCircleInfo} color="red" onClick={() => setShowNoAccessInfo(true)} />
                                            </td>}
                                    </>
                                    }

                                </tr>)}
                            </tbody>
                        </Table>
                        {reidentifications.length > itemsPerPage && <Row>
                            <Col className="d-grid gap-2">
                                <Button onClick={() => {
                                    if (pageNumber !== 0)
                                        setPageNumber(pageNumber - 1);
                                }}>
                                    <FontAwesomeIcon icon={faLeftLong} />
                                </Button>
                            </Col>
                            <Col>
                                <b>Page: {pageNumber + 1} of {Math.ceil(reidentifications.length / itemsPerPage)} </b>
                            </Col>
                            <Col className="d-grid gap-2">
                                <Button onClick={() => {
                                    if (pageNumber + 1 !== Math.ceil(reidentifications.length / itemsPerPage))
                                        setPageNumber(pageNumber + 1)
                                }}>

                                    <FontAwesomeIcon icon={faRightLong} />
                                </Button>
                            </Col>
                        </Row>
                        }
                    </>
                }
                <br />
                <div className="d-grid gap-2">
                    <Button size="sm" onClick={() => setShowHistory(!showHistory)}>{showHistory ? 'Hide' : 'Show'} reidentification history</Button>
                </div>
            </Toast.Body>
        </Toast>
        <Modal show={showNoAccessInfo} onHide={() => { setShowNoAccessInfo(false) }}>
            <Modal.Header closeButton>You do not have access to this Patient Record</Modal.Header>
            <Modal.Body>
                <p>You have tried to reidentify a patient's that our records indicate that you do not have access to.</p>
                <p>Check that the patient is registered to an GP practice that you have access to.</p>
                <p>To view your ODS organisations configured in the reidentification tool click <a href="about:blank">here</a> and contact your local ICB should you need further access.</p>
                <p>Any changes to the patient record regisistration will take 24 hours to apply to the reidentification service </p>
            </Modal.Body>
        </Modal>
    </ToastContainer>
}

export default ReportToast