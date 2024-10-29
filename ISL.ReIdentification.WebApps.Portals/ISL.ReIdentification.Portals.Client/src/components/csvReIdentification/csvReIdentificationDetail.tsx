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
                    <Alert variant="info">
                        <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation, 000000000000 will be returned if not found or no access found.</p>
                        <p><strong>Note:</strong> all reidentification requests are subject to breach monitoring and reporting</p>
                        <p>Details of breach thresholds can be found <a href="about:blank" target="blank" >here</a></p>
                    </Alert>
                </Row>
            </Container>
        </>
    );
};

export default CsvReIdentificationDetail;