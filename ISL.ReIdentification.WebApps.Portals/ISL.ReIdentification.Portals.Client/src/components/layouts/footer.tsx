import { faCopyright } from '@fortawesome/free-solid-svg-icons/faCopyright';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Col, Container, Row } from "react-bootstrap";

const FooterComponent: React.FC = () => {
    return (
        <Container>
            <Row className="bg-light">
                <Col className="m-2">
                    <FontAwesomeIcon icon={faCopyright} className="me-2 fa-icon" />
                    2024 NEL ICB. All rights reserved.
                </Col>
            </Row>
        </Container>
    );
}

export default FooterComponent;