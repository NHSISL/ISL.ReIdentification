import { faFileLines, faHome, faCog, faUser, faAddressBook, faUserDoctor, faChartBar,v } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { Link, useLocation } from 'react-router-dom';
import { FeatureDefinitions } from '../../featureDefinitions';
import { FeatureSwitch } from '../accessControls/featureSwitch';
import { SecuredComponent } from '../securitys/securedComponents';
import securityPoints from '../../securityMatrix';
import { SecuredLink } from '../securitys/securedLinks';
import { faUserFriends } from '@fortawesome/free-solid-svg-icons/faUserFriends';
import { faIdBadge } from '@fortawesome/free-solid-svg-icons/faIdBadge';

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

        </ListGroup>
    );
}

export default MenuComponent;