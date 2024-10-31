import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent } from "react";
import { Button, Table, Toast, ToastContainer } from "react-bootstrap";
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

const ReportToast : FunctionComponent<ReportToastProps> = (props) => {
    const {position, hidden, hide, clearList, reidentifications, lastSelectedPseudo } = props;



    return  <ToastContainer position={position || "bottom-end"} hidden={hidden || reidentifications.length===0} style={{marginTop:"33px"}}>
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
                        <td>{ri.pseudo}</td>
                        <td>
                            {ri.loading ?  <FontAwesomeIcon icon={faSpinner} pulse/> : <>{ri.nhsnumber}</> }
                        </td>
                        <td><CopyIcon content={ri.nhsnumber || ""} resetTime={1000}/></td>
                    </tr>)}
                </tbody>
            </Table>
        </Toast.Body>
    </Toast>
</ToastContainer>
}

export default ReportToast