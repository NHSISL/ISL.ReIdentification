import { FunctionComponent, useState } from "react";
import ReIdentificationDetailCard from "./reIdentificationDetailCard";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { Button, Container, Row, Spinner } from "react-bootstrap";
import { BreachModal } from "../breachDetails/BreachModal";

const ReIdentificationDetail: FunctionComponent = () => {
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [showBreachModal, setShowBreachModal] = useState(false);

    if (isLoading) {
        return <Spinner />
    }

    return (
        <>
            <div>
                <p>This page provides a simple reidentification for a single patient pseudo identifer</p>
                <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation.</p>
                <p><strong>Note:</strong> all reidentification requests are subject to breach monitoring and reporting</p>
                <p>Details of breach thresholds can be found <Button size="sm" variant="secondary" onClick={()=> setShowBreachModal(true)}>here</Button>.</p>
            </div>
            {mappedLookups &&
                <Container className="text-center">
                    <Row className="justify-content-md-center">
                        <ReIdentificationDetailCard
                            lookups={mappedLookups}
                        />
                    </Row>
                </Container>
            }
            <BreachModal show={showBreachModal} hide={() => setShowBreachModal(false)} />
        </>
    );
};

export default ReIdentificationDetail;