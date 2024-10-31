import { FunctionComponent } from "react";
import { Button } from "react-bootstrap";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";
import { Link } from "react-router-dom";

type CsvIdentificationRequestRowProps = {
    csvIdentificationRequest: CsvIdentificationRequest;
}

const CsvIdentificationRequestRow: FunctionComponent<CsvIdentificationRequestRowProps> = (props) => {
    const {
        csvIdentificationRequest
    } = props;


    return (
        <tr>
            <td>{csvIdentificationRequest.requesterDisplayName}</td>
            <td>{csvIdentificationRequest.requesterEmail}</td>
            <td>{csvIdentificationRequest.recipientDisplayName}</td>
            <td>{csvIdentificationRequest.recipientEmail}</td>
            <td>{csvIdentificationRequest.filepath}</td>
            <td>{csvIdentificationRequest.reason}</td>
            <td>
                <Link to={`/csvReIdentification/${csvIdentificationRequest.id}`} >
                    <Button size="sm" variant="link">
                        View to Download
                    </Button>
                </Link>
            </td>
        </tr>
    );
}

export default CsvIdentificationRequestRow;