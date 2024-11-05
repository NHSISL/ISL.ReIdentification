import { faHome, faPerson } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { ListGroup } from 'react-bootstrap';
import { useLocation } from 'react-router-dom';
import { SecuredLink } from '../securitys/securedLinks';
import { faListAlt } from '@fortawesome/free-solid-svg-icons/faListAlt';
import { faTable } from '@fortawesome/free-solid-svg-icons/faTable';


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
                className={`bg-dark text-white ${activePath === '/reports' ? 'active' : ''}`}
                onClick={() => handleItemClick('/report')}>
                <FontAwesomeIcon icon={faHome} className="me-2 fa-icon" />
                <SecuredLink to="/report">Report Reidentification</SecuredLink>
            </ListGroup.Item>

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/csvReIdentificationWorklist' ? 'active' : ''}`}
                onClick={() => handleItemClick('/csvReIdentificationWorklist')}>
                <FontAwesomeIcon icon={faTable} className="me-2 fa-icon" />
                <SecuredLink to="/csvReIdentificationWorklist">My CSV Worklist</SecuredLink>
            </ListGroup.Item>

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/reIdentification' ? 'active' : ''}`}
                onClick={() => handleItemClick('/reIdentification')}>
                <FontAwesomeIcon icon={faPerson} className="me-2 fa-icon" />
                <SecuredLink to="/reIdentification">Reidentify Single Patient</SecuredLink>
            </ListGroup.Item>

            <ListGroup.Item
                className={`bg-dark text-white ${activePath === '/csvReIdentification' ? 'active' : ''}`}
                onClick={() => handleItemClick('/csvReIdentification')}>
                <FontAwesomeIcon icon={faListAlt} className="me-2 fa-icon" />
                <SecuredLink to="/csvReIdentification">ReIdentify Dataset</SecuredLink>
            </ListGroup.Item>
        </ListGroup>
    );
}

export default MenuComponent;