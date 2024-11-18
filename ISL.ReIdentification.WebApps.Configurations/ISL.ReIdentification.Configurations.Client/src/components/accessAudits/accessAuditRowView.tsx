import { FunctionComponent } from "react";
import moment from "moment";
import { AccessAudit } from "../../models/accessAudit/accessAudit";

type AccessAuditRowProps = {
    accessAudit: AccessAudit;
};

const AccessAuditRowView: FunctionComponent<AccessAuditRowProps> = (props) => {
    const {
        accessAudit
    } = props
    return (
        <>
            <tr>
                <td>{accessAudit.pseudoIdentifier}</td>
                <td>{accessAudit.givenName} {accessAudit.surname}</td>
                <td>{accessAudit.email}</td>
                <td>{accessAudit.reason}</td>
                <td>{accessAudit.message}</td>
                <td>{accessAudit.organisation}</td>
                <td>{moment(accessAudit.createdDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            </tr>



        </>
    );
}

export default AccessAuditRowView;