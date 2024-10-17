import { Container, Row, Col } from "react-bootstrap";

export const Home = () => {
    return (
        <Container fluid className="mt-4">
            <Row className="mb-4">
                <Col>
                    <h1>Welcome to Re-Identification</h1>
                    <h2>User Portal</h2>
                </Col>
            </Row>
            <Row>
                <Col>
                </Col>
            </Row>
        </Container>
    );
}