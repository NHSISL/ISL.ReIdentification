import { FunctionComponent } from "react";
import { Link } from 'react-router-dom';
import { Button } from "react-bootstrap";
import { faCheck, faTimes } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";

type ImpersonationContextProjectRowProps = {
    impersonationContext: ImpersonationContext;
}

const ImpersonationContextProjectRow: FunctionComponent<ImpersonationContextProjectRowProps> = (props) => {
    const {
        impersonationContext
    } = props;


    return (
        <tr>
            <td>{impersonationContext.projectName}</td>
            <td>{impersonationContext.isApproved
                ? <FontAwesomeIcon icon={faCheck} className="text-success" />
                : <FontAwesomeIcon icon={faTimes} className="text-danger" />}
            </td>
            <td>
                <Link to={`/impersonationContextDetail/${impersonationContext.id}`}>
                    <Button onClick={() => { }}>
                        Details
                    </Button>
                </Link>
            </td>
        </tr>
    );
}

export default ImpersonationContextProjectRow;