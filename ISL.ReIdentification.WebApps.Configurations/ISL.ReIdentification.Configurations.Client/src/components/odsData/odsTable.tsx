import { FunctionComponent, useMemo, useState } from "react";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import OdsRow from "./odsRow";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { OdsDataView } from "../../models/views/components/odsData/odsDataView";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { odsDataViewService } from "../../services/views/odsData/odsDataViewService";

type OdsTableProps = object;

const OdsTable: FunctionComponent<OdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);


    const {
        mappedOdsData: odsRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage
    } = odsDataViewService.useGetAllOdsData(
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

    return (
        <>
            <SearchBase id="search" label="Search ods" value={searchTerm} placeholder="Search ODS Table"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
            <br />

            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> ODS Table</Card.Header>
                    <Card.Body>
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Organisation Name</th>
                                        <th>Organisation Code</th>
                                        <th>Has Children</th>
                                        <th>Relationship With Parent StartDate</th>
                                        <th>Relationship With Parent EndDate</th>
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
                                                {odsRetrieved?.map(
                                                    (odsDataView: OdsDataView) => (
                                                        <OdsRow
                                                            key={odsDataView.id}
                                                            ods={odsDataView}
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
                    </Card.Body>
                </Card>
            </Container>
        </>
    );
};

export default OdsTable;