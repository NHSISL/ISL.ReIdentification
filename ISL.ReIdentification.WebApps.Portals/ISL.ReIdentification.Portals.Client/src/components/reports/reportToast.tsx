import { faCircleInfo, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react";
import { Button, Modal, Table, Toast, ToastContainer } from "react-bootstrap";
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
    const { position, hidden, hide, clearList, reidentifications, lastSelectedPseudo } = props;
    const [showNoAccessInfo, setShowNoAccessInfo] = useState(false);



    return <ToastContainer position={position || "bottom-end"} hidden={hidden || reidentifications.length === 0} style={{ marginTop: "33px" }}>
        <Toast onClose={() => hide(true)}>
            <Toast.Header>
                <strong className="me-auto">Reidentification</strong>
                <Button size="sm" onClick={clearList}>Clear</Button>
            </Toast.Header>
            <Toast.Body>
                <Table size="sm" bordered>
                    <tbody>
                        {reidentifications.map((ri) => <tr key={ri.pseudo}
                            className={lastSelectedPseudo && ri.pseudo === lastSelectedPseudo ? "table-active" : ""}>

                            {ri.hasAccess ? <>
                                <td>{ri.pseudo}</td>
                                <td>
                                    {ri.loading ? <FontAwesomeIcon icon={faSpinner} pulse /> : <>{ri.nhsnumber}</>}
                                </td>
                                <td><CopyIcon content={ri.nhsnumber || ""} resetTime={1000} /></td>
                            </> : <>
                                <td>{ri.pseudo}</td>
                                <td colSpan={2}>NO ACCESS <FontAwesomeIcon icon={faCircleInfo} color="red" onClick={() => setShowNoAccessInfo(true)} /></td>
                            </>
                            }
                        </tr>)}
                    </tbody>
                </Table>
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