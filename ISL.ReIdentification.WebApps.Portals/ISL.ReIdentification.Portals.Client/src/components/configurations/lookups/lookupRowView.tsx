import React, { FunctionComponent } from "react";
import { LookupView } from "../../../models/views/components/lookups/lookupView";
import { SecuredComponent } from "../../securitys/securedComponents";
import { Button } from "react-bootstrap";
import moment from "moment";

interface LookupRowViewProps {
    lookup: LookupView;
    allowedToEdit: boolean;
    allowedToDelete: boolean;
    onEdit: (value: string) => void;
    onDelete: (value: string) => void;
}

const LookupRowView: FunctionComponent<LookupRowViewProps> = (props) => {
    const {
        lookup,
        allowedToEdit,
        allowedToDelete,
        onEdit,
        onDelete
    } = props;

    return (
        <tr>
            <td>{lookup.name}</td>
            <td>{lookup.value}</td>
            <td>{lookup.createdBy}</td>
            <td>{moment(lookup.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
                <td>
                    {allowedToEdit && (
                        <SecuredComponent>
                            <Button onClick={() => onEdit('EDIT')}>Edit</Button>
                        </SecuredComponent>
                    )}
                    &nbsp;
                    {allowedToDelete && (
                        <SecuredComponent>
                        <Button onClick={() => onDelete('DELETE')} variant="danger">Delete</Button>
                        </SecuredComponent>
                    )}
                </td>
        </tr>
    );
}

export default LookupRowView;