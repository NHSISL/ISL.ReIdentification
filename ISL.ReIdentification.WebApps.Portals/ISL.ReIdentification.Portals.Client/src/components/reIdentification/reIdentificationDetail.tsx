import { FunctionComponent, useState } from "react";
import ReIdentificationDetailCard from "./reIdentificationDetailCard";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { Alert, Col, Container, Row, Spinner } from "react-bootstrap";
import { BreachModal } from "../breachDetails/BreachModal";

const ReIdentificationDetail: FunctionComponent = () => {
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [showBreachModal, setShowBreachModal] = useState(false);

    if (isLoading) {
        return <Spinner />
    }

    return (
        <>
            {mappedLookups &&
                <>
                    <Container>
                        <Row className="justify-content-md-center">
                            <Col>
                            <ReIdentificationDetailCard
                                lookups={mappedLookups}
                            />
                            </Col>
                        </Row>
                    </Container>
                    <br />
                    <Container>
                        <Row className="justify-content-md-center">
                            <Alert variant="secondary" style={{ width: '50rem' }}>
                                <p>This page provides a simple reidentification for a single patient pseudo identifer</p>
                                <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation.</p>
                                <p><strong>Note:</strong> all reidentification requests are subject to breach monitoring and reporting</p>
                                <p>Details of breach thresholds can be found <a href='#' onClick={() => setShowBreachModal(true)}>here</a>.</p>
                            </Alert>
                        </Row>
                    </Container>
                </>
            }
            <BreachModal show={showBreachModal} hide={() => setShowBreachModal(false)} />
        </>
    );
};

export default ReIdentificationDetail;