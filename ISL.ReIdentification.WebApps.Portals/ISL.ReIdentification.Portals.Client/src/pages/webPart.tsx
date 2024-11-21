import { FunctionComponent } from "react"
import { useParams } from "react-router-dom"
import ReidentificationWebPart from "../components/reIdentification/reidentificationWebPart";
import { Button, Col, Container, Row } from "react-bootstrap";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import LoginUnAuthorisedComponent from "../components/layouts/loginUnauth";

export const WebPart: FunctionComponent = () => {
    const { pseudoId } = useParams();
    return <Container className="h-100">
        <Row className="align-items-center h-100">
            <Col className="d-flex justify-content-center">
                <UnauthenticatedTemplate>
                    <LoginUnAuthorisedComponent />
                </UnauthenticatedTemplate>
                <AuthenticatedTemplate>
                    <ReidentificationWebPart pseudo={pseudoId} />
                </AuthenticatedTemplate>
            </Col>
        </Row>
    </Container>
}