import { FunctionComponent, ReactElement } from "react";
import { ReIdRecord } from "../../types/ReIdRecord"
import { Alert, Card, CardFooter } from "react-bootstrap";
import CopyIcon from "../core/copyIcon";

type ReidentificationResultViewProps = {
    reidentificationRecord: ReIdRecord;
    children?: ReactElement
}
const ReidentificationResultView: FunctionComponent<ReidentificationResultViewProps> = ({ reidentificationRecord, children }) => {

    if (reidentificationRecord.hasAccess) {
        return <>
            <Card>
                <Card.Body>
                    <Alert variant="success">
                        NHS Number: {reidentificationRecord.nhsnumber}&nbsp;<CopyIcon content={reidentificationRecord.nhsnumber} />
                    </Alert>
                </Card.Body>
                {children &&
                    <CardFooter>
                        {children}
                    </CardFooter>
                }
            </Card>
        </>
    }

    return <>
        <Card.Header><h4>Reidentification not allowed.</h4></Card.Header>
        <Card.Body>
            <Alert variant="danger">
                <p>You have tried to reidentify a patient's that our records indicate that you do not have access to.</p>
                <p>Check that the patient is registered to an GP practice that you have access to.</p>
                <p>To view your ODS organisations configured in the reidentification tool click <a href="about:blank">here</a> and contact your local ICB should you need further access.</p>
                <p>Any changes to the patient record regisistration will take 24 hours to apply to the reidentification service </p>
            </Alert>
        </Card.Body>
        {children && <CardFooter>
            {children}
            </CardFooter>
        }
    </>

}

export default ReidentificationResultView;