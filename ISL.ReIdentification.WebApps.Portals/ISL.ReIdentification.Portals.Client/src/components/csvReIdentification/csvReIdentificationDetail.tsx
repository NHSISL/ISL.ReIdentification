import { FunctionComponent } from "react";
import { Container, Row, Alert } from "react-bootstrap";
import CsvReIdentificationDetailCard from "./csvReIdentificationDetailCard";

const CsvReIdentificationDetail: FunctionComponent = () => {
    
    return (
        <>
            <Container className="">
                <Row className="justify-content-md-center">
                    <CsvReIdentificationDetailCard />
                </Row>
            </Container>
            <br />
            <Container className="">
                <Row className="justify-content-md-center">
                    <Alert variant="secondary" style={{ width: '50rem' }}>
                        <p>
                            <strong>Note:</strong> You will only be able to reidentify patients that are present within
                            <strong> your</strong> organisation, '0000000000' will be returned if access is not found for a patient.
                        </p>
                        <p><strong>Note:</strong>All Reidentification requests are subject to breach monitoring and reporting.</p>
                        <p>Details of breach thresholds can be found <a href="about:blank" target="blank" >here</a></p>
                    </Alert>
                </Row>
            </Container>
        </>
    );
};

export default CsvReIdentificationDetail;