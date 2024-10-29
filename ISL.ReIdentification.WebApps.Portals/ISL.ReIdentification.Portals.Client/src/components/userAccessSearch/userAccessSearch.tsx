import { debounce } from "lodash";
import { FunctionComponent, useMemo, useState } from "react";
import { Button, Form, FormGroup, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTimes } from "@fortawesome/free-solid-svg-icons";
import { userAccessService } from "../../services/foundations/userAccessService";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";

type UserAccessSearchProps = {
    selectUser: (value: UserAccessView) => void;
};

const UserAccessSearch: FunctionComponent<UserAccessSearchProps> = ({ selectUser }) => {
    const [searchTerm, setSearchTerm] = useState<string>();
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
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

    return (
        <>
            <FormGroup>
                <Form.Label className="text-start">Recipient Email Address</Form.Label>
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
            </FormGroup>

            <div style={{ paddingTop: "10px" }}>
                <Table size="sm" striped hover>
                    <tbody>
                        {data && data.map((userAccess: UserAccessView) => (
                            <tr onClick={() => selectUser(userAccess)} key={userAccess.id}>
                                <td><small>{userAccess.displayName}</small></td>
                                <td><small>{userAccess.email}</small></td>
                                <td><small>{userAccess.jobTitle}</small></td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            </div>
        </>
    );
};

export default UserAccessSearch;