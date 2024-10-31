import { FunctionComponent } from "react";
import { Button } from "react-bootstrap";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";

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
                <Button variant="link" onClick={() => { }}>
                    View to Download
                </Button>
            </td>
        </tr>
    );
}

export default CsvIdentificationRequestRow;