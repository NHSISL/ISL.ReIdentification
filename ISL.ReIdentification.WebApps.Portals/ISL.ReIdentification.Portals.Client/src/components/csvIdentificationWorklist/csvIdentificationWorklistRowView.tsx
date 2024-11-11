import { FunctionComponent } from "react";
import { Button } from "react-bootstrap";
import { CsvIdentificationRequest } from "../../models/csvIdentificationRequest/csvIdentificationRequest";
import { Link } from "react-router-dom";
import moment from "moment";

type CsvIdentificationWorklistRowProps = {
    csvIdentificationRequest: CsvIdentificationRequest;
}

const CsvIdentificationWorklistRow: FunctionComponent<CsvIdentificationWorklistRowProps> = (props) => {
    const {
        csvIdentificationRequest
    } = props;


    return (
        <tr>
            <td>{csvIdentificationRequest.requesterDisplayName}</td>
            <td>{csvIdentificationRequest.requesterEmail}</td>
            <td>{csvIdentificationRequest.recipientDisplayName}</td>
            <td>{csvIdentificationRequest.recipientEmail}</td>
            <td>{csvIdentificationRequest.reason}</td>
            <td>{ moment(csvIdentificationRequest.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
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

export default CsvIdentificationWorklistRow;