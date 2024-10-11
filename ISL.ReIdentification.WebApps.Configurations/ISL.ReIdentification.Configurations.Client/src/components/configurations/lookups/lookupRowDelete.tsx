import React, { FunctionComponent } from "react";
import { Guid } from "guid-typescript";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { toastSuccess } from "../../../brokers/toastBroker";
import { Button } from "react-bootstrap";

interface LookupRowDeleteProps {
    lookup: LookupView;
    onCancel: (id: Guid) => void;
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

