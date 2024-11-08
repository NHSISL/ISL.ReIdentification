import { faHome, faCog, faUser, faAddressBook, faUserDoctor } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';
import { SecuredLink } from '../securitys/securedLinks';
import { faUserFriends } from '@fortawesome/free-solid-svg-icons/faUserFriends';
import { faIdBadge } from '@fortawesome/free-solid-svg-icons/faIdBadge';
import { FeatureDefinitions } from '../../featureDefinitions';
import { FeatureSwitch } from '../accessControls/featureSwitch';

const MenuComponent: React.FC = () => {
    const location = useLocation();
    const [activePath, setActivePath] = useState(location.pathname);

    const handleItemClick = (path: string) => {
        setActivePath(path);
    };

    return (

        <ListGroup variant="flush" className="text-start border-0">
            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/' ? 'active' : ''}`}
                onClick={() => handleItemClick('/')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                <SecuredLink to="/home">Home</SecuredLink>
            </ListGroup.Item>

            <FeatureSwitch feature={FeatureDefinitions.UserAccess}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/userAccess' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/userAccess')}>
                    <FontAwesomeIcon icon={faUser} className="me-2 fa-icon" />
                    <SecuredLink to="/userAccess">User Access</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            {/*<ListGroup.Item*/}
            {/*    className={`bg-dark text-white ${activePath === '/impersonationContext' ? 'active' : ''}`}*/}
            {/*    onClick={() => handleItemClick('/impersonationContext')}>*/}
            {/*    <FontAwesomeIcon icon={faUserFriends} className="me-2 fa-icon" />*/}
            {/*    <SecuredLink to="/impersonationContext">Impersonation Context</SecuredLink>*/}
            {/*</ListGroup.Item>*/}

            {/*<ListGroup.Item*/}
            {/*    className={`bg-dark text-white ${activePath === '/csvIdentificationRequest' ? 'active' : ''}`}*/}
            {/*    onClick={() => handleItemClick('/csvIdentificationRequest')}>*/}
            {/*    <FontAwesomeIcon icon={faIdBadge} className="me-2 fa-icon" />*/}
            {/*    <SecuredLink to="/csvIdentificationRequest">Csv Identification Request</SecuredLink>*/}
            {/*</ListGroup.Item>*/}

            <FeatureSwitch feature={FeatureDefinitions.Pds}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/pdsData' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/pdsData')}>
                    <FontAwesomeIcon icon={faAddressBook} className="me-2 fa-icon" />
                    <SecuredLink to="/pdsData">PDS Data View</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Ods}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/odsData' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/odsData')}>
                    <FontAwesomeIcon icon={faUserDoctor} className="me-2 fa-icon" />
                    <SecuredLink to="/odsData">Ods Data View</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>

            <FeatureSwitch feature={FeatureDefinitions.Configuration}>
                <ListGroup.Item
                    className={`bg-dark text-white ${activePath === '/configuration/home' ? 'active' : ''}`}
                    onClick={() => handleItemClick('/configuration/home')}>
                    <FontAwesomeIcon icon={faCog} className="me-2 fa-icon" />
                    <SecuredLink to="/configuration/home">Configuration</SecuredLink>
                </ListGroup.Item>
            </FeatureSwitch>
        </ListGroup>
    );
}

export default MenuComponent;