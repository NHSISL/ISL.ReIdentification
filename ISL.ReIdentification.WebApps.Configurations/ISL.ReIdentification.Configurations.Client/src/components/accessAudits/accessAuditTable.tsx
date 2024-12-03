import { FunctionComponent, useMemo, useState } from "react";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import AccessAuditRow from "./accessAuditRow";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { AccessAudit } from "../../models/accessAudit/accessAudit";
import { accessAuditViewService } from "../../services/views/accessAudit/accessAuditViewService";

type AccessAuditTableProps = object;

const AccessAuditTable: FunctionComponent<AccessAuditTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);


    const {
        mappedAccessAudit: accessAuditRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        totalPages
    } = accessAuditViewService.useGetAllAccessAuditData(
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
        return hasNextPage;
    };

    return (
        <>
            <SearchBase id="search" label="Search accessAudit" value={searchTerm} placeholder="Search Access Audit Table"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
            <br />

            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> Access Audit Table</Card.Header>
                    <Card.Body>
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                       {/* <th>Identifier</th>*/}
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Reason</th>
                                        <th>Message</th>
                                        <th>Created Date</th>
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
                                            {accessAuditRetrieved?.map(
                                                (accessAudit: AccessAudit) => (
                                                    <AccessAuditRow
                                                        key={accessAudit.id}
                                                        accessAudit={accessAudit}
                                                    />
                                                )
                                            )}
                                            <tr>
                                                <td colSpan={7} className="text-center">
                                                    <InfiniteScrollLoader
                                                            loading={isFetchingNextPage}
                                                            spinner={<SpinnerBase />}
                                                            noMorePages={!hasNoMorePages()}
                                                            noMorePagesMessage={<>-- No more pages --</>}
                                                            totalPages={totalPages}
                                                    />
                                                </td>
                                            </tr>
                                        </>
                                    )}
                                </tbody>
                            </Table>
                        </InfiniteScroll>
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default AccessAuditTable;