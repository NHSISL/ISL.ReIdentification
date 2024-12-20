import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { Button, Container, Navbar } from "react-bootstrap";
import Login from '../securitys/login';
import { useFrontendConfiguration } from '../../hooks/useFrontendConfiguration';

interface NavbarComponentProps {
    toggleSidebar: () => void;
    showMenuButton: boolean;
}

const NavbarComponent: React.FC<NavbarComponentProps> = ({ toggleSidebar, showMenuButton }) => {

    const { configuration } = useFrontendConfiguration()

    return (
        <Navbar sticky="top" style={{ backgroundColor: configuration?.bannerColour || "#f8f9fa" }}>
            <Container fluid>
                {showMenuButton && (
                    <Button onClick={toggleSidebar} variant="outline-dark" className="ms-3">
                        <FontAwesomeIcon icon={faBars} />
                    </Button>
                )}
                <Navbar.Brand href="/" className="me-auto ms-3 d-flex align-items-center">
                    <img src="/LHDLogoRound.png" alt="London Data Service logo" height="30" width="30" />
                    <span className="d-none d-md-inline" style={{ marginLeft: "10px" }}>
                        LDS - Re-Identification Portal
                        {configuration?.environment !== "Live" && <>&nbsp;({configuration?.environment})</>}
                    </span>
                </Navbar.Brand>
                <Navbar.Text>
                    <Login />
                </Navbar.Text>
            </Container>
        </Navbar>
    );
}

export default NavbarComponent;