import { FunctionComponent } from "react"
import { useParams } from "react-router-dom"
import ReidentificationWebPart from "../components/reIdentification/reidentificationWebPart";
import { Col, Container, Row } from "react-bootstrap";

export const WebPart: FunctionComponent = () => {
    const { pseudoId } = useParams();
    return <Container className="h-100">
        <Row className="align-items-center h-100">
            <Col className="d-flex justify-content-center">
                <ReidentificationWebPart pseudo={pseudoId} />
            </Col>
        </Row>
    </Container>
}