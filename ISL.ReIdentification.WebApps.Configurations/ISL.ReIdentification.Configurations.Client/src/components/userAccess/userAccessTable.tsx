import { FunctionComponent } from "react";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Button, Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase } from "@fortawesome/free-solid-svg-icons";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { Link } from "react-router-dom";
import securityPoints from "../../securityMatrix";
import { SecuredComponent } from "../securitys/securedComponents";

type UserAccessTableProps = object;

const UserAccessTable: FunctionComponent<UserAccessTableProps> = () => {
    const {
        isLoading,
        data
    } = userAccessViewService.useGetDistinctUsers();

    return (
        <>
            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> User Access</Card.Header>
                    <Card.Body>

                        <Table striped bordered hover variant="light">
                            <thead>
                                <tr>
                                    <th>Display Name</th>
                                    <th>Email</th>
                                    <th>Action(s)</th>
                                </tr>
                            </thead>
                            <tbody>
                                {isLoading ? (
                                    <tr>
                                        <td colSpan={6} className="text-center">
                                            <SpinnerBase />
                                        </td>
                                    </tr>
                                ) : (
                                    <>
                                        {data && data?.map(d => <tr key={d.entraUserId}>
                                            <td>{d.displayName}</td>
                                            <td>{d.email}</td>
                                            <td>
                                                <SecuredComponent allowedRoles={securityPoints.userAccess.edit}>
                                                    <Link to={`/userAccess/${d.entraUserId}`} >
                                                        <Button size="sm">
                                                            Edit
                                                        </Button>
                                                    </Link>
                                                </SecuredComponent>
                                            </td>
                                        </tr>)}
                                    </>
                                )}
                            </tbody>
                        </Table>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default UserAccessTable;