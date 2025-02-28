import { faCopyright } from '@fortawesome/free-solid-svg-icons/faCopyright';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Col, Container, Row } from "react-bootstrap";
import { OdsLoadAudit } from '../audit/odsLoadAudit';
import { PdsLoadAudit } from '../audit/pdsLoadAudit';

const FooterComponent: React.FC = () => {
    return (
        <Container>
            <Row className="bg-dark">
                <Col className="mb-2">
                    <small>
                        <OdsLoadAudit isAlert={true} />
                        <PdsLoadAudit isAlert={true} />
                    </small>
                </Col>
            </Row>

            <Row className="bg-dark">
                <Col>
                    <hr style={{ borderColor: 'white' }} className="m-0 mt-2"/>
                </Col>
            </Row>

            <Row className="bg-dark text-white">
                <Col className="m-2">
                    <small>
                        <FontAwesomeIcon icon={faCopyright} className="me-2 fa-icon fa-regular" />
                        2025 One London. All rights reserved.
                    </small>
                </Col>
            </Row>
        </Container>
    );
}

export default FooterComponent;