import  debounce from "lodash/debounce";
import { FunctionComponent, useMemo, useState } from "react";
import { Button, Card, Form, FormGroup, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase, faTimes } from "@fortawesome/free-solid-svg-icons";
import { entraUsersService } from "../../services/foundations/entraUsersService";
import { entraUser } from "../../models/views/components/entraUsers/entraUsers";

type EntraUserSearchProps = {
    selectUser : (value: entraUser) => void;
};

const EntraUserSearch: FunctionComponent<EntraUserSearchProps> = ({ selectUser }) => {
    const [searchTerm, setSearchTerm] = useState<string>();
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const { data } = entraUsersService.useSearchEntraUsers(debouncedTerm);

    const handleSearchChange = (value: string) => {
        setSearchTerm(value);
        handleDebounce(value);
    };

    const handleDebounce = useMemo(
        () =>
            debounce((value: string) => {
                setDebouncedTerm(value);
            }, 500),
        []
    );

    const clearSearch = () => {
        handleSearchChange("");
    }

    return (
        <Card>
            <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" />Add User</Card.Header>
            <Card.Body>
                <Form>
                    
                        <FormGroup>
                            <Form.Label>Email Address</Form.Label>
                            <InputGroup>
                                <Form.Control autoComplete="off" type="text" placeholder="Enter email address" onChange={(e) => handleSearchChange(e.target.value)} value={searchTerm}/>
                                <Button disabled={(!data || data.length === 0 ) && !searchTerm } onClick={clearSearch}><FontAwesomeIcon icon={faTimes} /></Button>
                            </InputGroup>
                        </FormGroup>
                    <div style={{paddingTop:"10px"}}>
                        <Table size="sm" bordered hover>
                            <tbody>
                                {data && data.map((user: entraUser) => ( 
                                    <tr onClick={() => selectUser(user)} key={user.id}>
                                        <td>{user.mail}</td>
                                        <td>{user.displayName}</td>
                                        <td>{user.jobTitle}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>
                    </div>
                </Form>
            </Card.Body>
        </Card>
    );
};

export default EntraUserSearch;