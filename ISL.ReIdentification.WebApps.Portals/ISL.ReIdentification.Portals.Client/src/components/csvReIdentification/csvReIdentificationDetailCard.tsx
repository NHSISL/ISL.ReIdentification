import { FunctionComponent } from "react";
import { Card } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import ReIdentificationDetailCardView from "../reIdentification/reIdentificationDetailCardView";
import CsvReIdentificationDetailCardView from "./csvReIdentificationDetailCardView";

interface CsvReIdentificationDetailCardProps {
    lookups: Array<LookupView>;
}

const CsvReIdentificationDetailCard: FunctionComponent<CsvReIdentificationDetailCardProps> = (props) => {
    const {
        lookups,
    } = props;

    return (
        <Card style={{ width: '25rem' }}>
            <Card.Body>
                <CsvReIdentificationDetailCardView
                    lookups={lookups}
                />
            </Card.Body>
        </Card>
    );
};

export default CsvReIdentificationDetailCard;