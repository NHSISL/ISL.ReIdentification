import { Container, Row, Col } from "react-bootstrap";

export const Home = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4 p-2">
                <Col>
                    <h1 className="display-5">Welcome to the One London Re-Identification Portal</h1>
                    <br />
                    <p>
                        Introducing the new One London Re-Identification platform from One London,
                        built to support Integrated Care Boards (ICBs). This service enables bulk re-identification of
                        patient records, integrating with BI tools for analysts and data teams.
                    </p>
                    <p>
                        The platform uses a permission-based model to verify user-patient relationships, cross-checking
                        with patient data from the Personal Demographic Service (PDS). It ensures privacy by redacting
                        patient details when access is not authorised, maintaining compliance and security.
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