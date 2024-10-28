import { FunctionComponent } from "react";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { Button } from "react-bootstrap";
import { toastSuccess } from "../../../brokers/toastBroker.success";

interface LookupRowDeleteProps {
    lookup: LookupView;
    onCancel: (id: string) => void;
    onDelete: (lookup: LookupView) => void;
}

const LookupRowDelete: FunctionComponent<LookupRowDeleteProps> = (props) => {
    const {
        lookup,
        onCancel,
        onDelete
    } = props;

    const handleDelete = (lookup: LookupView) => {
        onDelete(lookup);
        toastSuccess(`${lookup.name} Deleted`);
    }

    return (
        <tr>
            <td>
                <b>{lookup.name}</b>
            </td>
            <td>
                {lookup.value}
            </td>
            <td></td>
            <td></td>
            <td>
                <Button onClick={() => onCancel(lookup.id)} variant="warning">Cancel</Button>&nbsp;
                <Button onClick={() => handleDelete(lookup)} variant="danger">Yes, Delete</Button>
            </td>
        </tr>
    );
}

export default LookupRowDelete;

