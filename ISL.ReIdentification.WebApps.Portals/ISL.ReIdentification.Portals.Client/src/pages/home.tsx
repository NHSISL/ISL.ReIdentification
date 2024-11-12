import { Container, Row, Col } from "react-bootstrap";

export const Home = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4">
                <Col>
                    <h2>Welcome to Re-Identification</h2>
                    <p>
                        This service will provide capabilities to end users by integrating closely with Power BI
                        and tools for analysts and data engineering teams to allow bulk re-identification of
                        patient records.
                    </p>
                    <p>
                        The service will be responsible for checking the legitimate relationship between the
                        user and the patient. This will be achieved by providing a permission-based model
                        that validates the patient demographic record from the Personal Demographic
                        Service (PDS) held by ISL and the Organisation that the user is assigned to.
                        Further details of this permission model can be found in section XXX.
                        Reidentification permissions are checked for each patient in each request.
                        Any request for non-permissioned patients will return a record with redacted
                        information.
                    </p>
                </Col>
            </Row>
            <Row>
                <Col>
                </Col>
            </Row>
        </Container>
    );
}