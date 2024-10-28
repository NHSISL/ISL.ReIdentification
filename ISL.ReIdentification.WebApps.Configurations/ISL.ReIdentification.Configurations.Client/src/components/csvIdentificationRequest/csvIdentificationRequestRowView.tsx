import { FunctionComponent } from "react";
import { Link } from 'react-router-dom';
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
            <td>{csvIdentificationRequest.requesterFirstName}</td>
            <td>{csvIdentificationRequest.requesterLastName}</td>
            <td>{csvIdentificationRequest.requesterEmail}</td>
            <td>{csvIdentificationRequest.recipientFirstName}</td>
            <td>{csvIdentificationRequest.recipientLastName}</td>
            <td>{csvIdentificationRequest.reason}</td>
           
            <td>
                <Link to={`/csvIdentificationRequestDetail/${csvIdentificationRequest.id}`}>
                    <Button onClick={() => { }}>
                        Details
                    </Button>
                </Link>
            </td>
        </tr>
    );
}

export default CsvIdentificationRequestRow;