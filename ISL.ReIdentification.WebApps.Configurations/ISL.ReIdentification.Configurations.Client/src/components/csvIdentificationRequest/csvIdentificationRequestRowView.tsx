import { FunctionComponent } from "react";
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
        </tr>
    );
}

export default CsvIdentificationRequestRow;