import { debounce } from "lodash";
import { FunctionComponent, useMemo, useState } from "react";
import { Button, Form, FormGroup, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { userAccessService } from "../../services/foundations/userAccessService";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";

type UserAccessSearchProps = {
    selectUser: (value: UserAccessView) => void;
    labelText: string;
};

const UserAccessSearch: FunctionComponent<UserAccessSearchProps> = ({ selectUser, labelText }) => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [selectedUserName, setSelectedUserName] = useState<string | null>(null);
    const { data } = userAccessService.useSearchUserAccess(debouncedTerm);

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

    const handleUserSelect = (userAccess: UserAccessView) => {
        selectUser(userAccess);
        setSelectedUserName(userAccess.email);
        setSearchTerm("");
    }

    const clearSelectedUser = () => {
        setSelectedUserName(null);
    }

    return (
        <>
            <FormGroup>
                <Form.Label className="text-start"><strong>{labelText}</strong></Form.Label>
                <InputGroup>
                    <Form.Control
                        autoComplete="off"
                        type="text"
                        placeholder="Enter user email address"
                        onChange={(e) => handleSearchChange(e.target.value)}
                        value={searchTerm} />

                    <Button
                        disabled={(!data || data.length === 0) && !searchTerm}
                        onClick={clearSearch}>
                        <FontAwesomeIcon icon={faTimes} />
                    </Button>
                </InputGroup>

                <Form.Text className="text-muted">
                    Please select a Recipient, <strong>NOTE:</strong> this user must have an account in the ReIdentification Portal.
                </Form.Text>
            </FormGroup>

            {selectedUserName ? (
                <div>
                    <small>Selected User: <span className="text-success">{selectedUserName}</span></small>
                    <Button variant="link" onClick={clearSelectedUser}>
                        <FontAwesomeIcon icon={faTimes} style={{ color: "red" }} />
                    </Button>
                </div>
            ) : (
                <div style={{ paddingTop: "10px" }}>
                    <Table size="sm" striped hover>
                        <tbody>
                            {data && data.map((userAccess: UserAccessView) => (
                                <tr onClick={() => handleUserSelect(userAccess)} key={userAccess.id}>
                                    <td><small>{userAccess.displayName}</small></td>
                                    <td><small>{userAccess.email}</small></td>
                                    <td><small>{userAccess.jobTitle}</small></td>
                                    <td>
                                        <Button
                                            size="sm"
                                            variant="link"
                                            onClick={() => handleUserSelect(userAccess)}
                                            key={userAccess.id}>
                                            Select
                                        </Button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </Table>
                </div>
            )}
        </>
    );
};

export default UserAccessSearch;