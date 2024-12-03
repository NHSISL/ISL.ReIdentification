import { FunctionComponent } from "react";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";
import moment from "moment";

type PdsRowProps = {
    pds: PdsDataView;
};

const PdsRow: FunctionComponent<PdsRowProps> = (props) => {
    const {
        pds
    } = props
    return (
        <>
            <tr>
                <td>{pds.pseudoNhsNumber}</td>
                <td>{pds.orgCode}</td>
                <td>{moment(pds.relationshipWithOrganisationEffectiveFromDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
                <td>{moment(pds.relationshipWithOrganisationEffectiveToDate?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            </tr>



        </>
    );
}

export default PdsRow;