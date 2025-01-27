import { FunctionComponent, useMemo, useState } from "react";
import { Button, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { faRefresh } from "@fortawesome/free-solid-svg-icons";
import UserAccessRow from "./userAccessRow";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { SecuredComponent } from "../securitys/securedComponents";
import { Link } from "react-router-dom";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import securityPoints from "../../securityMatrix";

type OdsTableProps = object;

const OdsTable: FunctionComponent<OdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);

    const {
        mappedUserAccess: userAccessRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
    } = userAccessViewService.useGetDistinctUsersOdata(
        debouncedTerm
    );

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

    const hasNoMorePages = () => {
        return !hasNextPage;
    };

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };

    return (
        <>
            <InputGroup className="mb-3">
                <SearchBase id="search" label="Search users" value={searchTerm} placeholder="Search User Table"
                    onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>

            <SecuredComponent allowedRoles={securityPoints.userAccess.add}>
                <Link to="/userAccess/newUser">
                    <Button><FontAwesomeIcon icon={faDatabase} className="me-2" /> Add New User</Button>
                </Link>
            </SecuredComponent>

            <Container fluid className="infiniteScrollContainer ms-0 me-0 p-0 mt-4">
                <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                    <Table striped bordered hover variant="light" responsive>
                        <thead>
                            <tr>
                                <th>Display Name</th>
                                <th>Email</th>
                                <th>Action(s)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {isLoading || showSpinner ? (
                                <tr>
                                    <td colSpan={6} className="text-center">
                                        <SpinnerBase />
                                    </td>
                                </tr>
                            ) : (
                                <>
                                    {userAccessRetrieved?.map(
                                        (userAccessView: UserAccessView) => (
                                            <UserAccessRow
                                                key={userAccessView.id.toString()}
                                                userAccess={userAccessView}
                                            />
                                        )
                                    )}
                                    <tr>
                                        <td colSpan={7} className="text-center">
                                            <InfiniteScrollLoader
                                                loading={isLoading || isFetchingNextPage}
                                                spinner={<SpinnerBase />}
                                                noMorePages={hasNoMorePages()}
                                                noMorePagesMessage={<>-- No more pages --</>}
                                            />
                                        </td>
                                    </tr>
                                </>
                            )}
                        </tbody>
                    </Table>
                </InfiniteScroll>
            </Container>
        </>
    );
};

export default OdsTable;