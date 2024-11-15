import React from 'react';
import { useMsal } from "@azure/msal-react";
import { Button, Container, Row, Col, Card } from "react-bootstrap";
import { loginRequest } from '../../authConfig';

export const LoginUnAuthorisedComponent = () => {

    const { instance } = useMsal();

    const handleLoginRedirect = () => {
        instance.loginPopup(loginRequest).catch((error) => console.log(error));
    };

    return (
        <Container className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
            <Row>
                <Col>
                    <Card className="text-center" style={{ maxWidth: '400px', margin: 'auto' }}>
                        <Card.Body>
                            <Card.Title className="mb-4 ">
                                <img src="/LHDLogoRound.png" alt="London Data Service logo" height="50" width="50" />
                                <br />
                                <span style={{ marginLeft: "10px" }}>
                                    London Data Service <br />
                                    <strong className="hero-text"> Re-Identification</strong>
                                </span>

                            </Card.Title>
                            <Card.Text className="mb-4 align-items-left" >
                                <p>Welcome to the Lodon Data Service Reidentification Configuration Portal.</p>
                                <p>Please sign in to continue.</p>
                            </Card.Text>
                            <Button onClick={handleLoginRedirect} className="me-3">Sign in</Button>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}

export default LoginUnAuthorisedComponent;