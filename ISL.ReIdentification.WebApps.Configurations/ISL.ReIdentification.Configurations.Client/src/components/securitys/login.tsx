import React from 'react';
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { Button, NavDropdown } from "react-bootstrap";
// @ts-expect-error auth config from js file
import { loginRequest } from '../../authConfig';
import { UserProfile } from '../securitys/userProfile';

const Login: React.FC = () => {
    const { instance } = useMsal();
    const activeAccount = instance.getActiveAccount();

    const handleLogoutRedirect = () => {
        instance.logoutPopup().catch((error) => console.log(error));
    };

    const handleLoginRedirect = () => {
        instance.loginPopup(loginRequest).catch((error) => console.log(error));
    };

    return (
        <>
            <UnauthenticatedTemplate>
                <div className="collapse navbar-collapse justify-content-end">
                    <Button onClick={handleLoginRedirect} className="me-3">Sign in</Button>
                </div>
            </UnauthenticatedTemplate>
            <AuthenticatedTemplate>
                <NavDropdown title={activeAccount?.username} id="collasible-nav-dropdown" className="me-3">
                    <NavDropdown.Item onClick={handleLogoutRedirect}>Sign out</NavDropdown.Item>
                    <UserProfile />
                </NavDropdown>
            </AuthenticatedTemplate>
        </>
    );
};

export default Login;