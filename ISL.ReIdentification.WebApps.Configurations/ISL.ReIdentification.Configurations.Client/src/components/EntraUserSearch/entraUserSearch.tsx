import { debounce } from "lodash";
import React, { FunctionComponent, useMemo, useState } from "react";
import { Button, Card, Form, FormGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase } from "@fortawesome/free-solid-svg-icons";
import { entraUsersService } from "../../services/foundations/entraUsersService";
import { entraUser } from "../../models/views/components/entraUsers/entraUsers";

type EntraUserSearchProps = {
    selectUser : (value: entraUser) => void;
};

const EntraUserSearch: FunctionComponent<EntraUserSearchProps> = ({ selectUser }) => {
    const [searchTerm, setSearchTerm] = useState<string>();
    const [debouncedTerm, setDebouncedTerm] = useState<string>();
    const { data } = entraUsersService.useSearchEntraUsers(debouncedTerm);

    const handleSearchChange = (value: string) => {
        setSearchTerm(value);
        handleDebounce(value);
    };

    const handleDebounce = useMemo(
        () =>
            debounce((value: string) => {
                if(value.length) {
                    setDebouncedTerm(value);
                }
            }, 500),
        []
    );

    return (
        <Card>
            <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" />Ingestion Tracking</Card.Header>
            <Card.Body>
                <Form>
                    <FormGroup>
                        <Form.Label>Email Address</Form.Label>
                        <Form.Control autoComplete="off" type="text" placeholder="Enter email address" onChange={(e) => handleSearchChange(e.target.value)} value={searchTerm}/>
                    </FormGroup>
                    <div>SEARCH TERM: {debouncedTerm}</div>
                    <div>
                        <Table>
                            {data && data.map((user: entraUser) => ( 
                                <tr>
                                    <td>{user.displayName}</td>
                                    <td><Button size="sm" onClick={() => selectUser(user)}>Select</Button></td>
                                </tr>
                            ))}
                        </Table>
                    </div>
                </Form>
            </Card.Body>
        </Card>
    );
};

export default EntraUserSearch;