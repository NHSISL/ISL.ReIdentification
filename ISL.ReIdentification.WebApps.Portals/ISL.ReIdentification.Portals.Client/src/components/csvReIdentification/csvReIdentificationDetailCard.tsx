import { FunctionComponent } from "react";
import { Card } from "react-bootstrap";
import CsvReIdentificationDetailCardView from "./csvReIdentificationDetailCardView";

const CsvReIdentificationDetailCard: FunctionComponent = () => {

    return (
        <Card style={{ width: '50rem' }}>
            <Card.Body>
                <CsvReIdentificationDetailCardView/>
            </Card.Body>
        </Card>
    );
};

export default CsvReIdentificationDetailCard;