import { faHome } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';
import { SecuredLink } from '../securitys/securedLinks';


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

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/reIdentification' ? 'active' : ''}`}
                onClick={() => handleItemClick('/reIdentification')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                <SecuredLink to="/reIdentification">Product 4 - Simple Re-Id</SecuredLink>
            </ListGroup.Item>

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/csvReIdentification' ? 'active' : ''}`}
                onClick={() => handleItemClick('/csvReIdentification')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                <SecuredLink to="/csvReIdentification">Product 3 - CSV Upload</SecuredLink>
            </ListGroup.Item>

        </ListGroup>
    );
}

export default MenuComponent;