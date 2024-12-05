import { FunctionComponent, useMemo, useState } from "react";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import PdsRow from "./pdsRow";
import { pdsDataViewService } from "../../services/views/pdsData/pdsDataViewService";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import { faDatabase } from "@fortawesome/free-solid-svg-icons/faDatabase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";

type PdsTableProps = object;

const PdsTable: FunctionComponent<PdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);


    const {
        mappedPdsData: pdsRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
    } = pdsDataViewService.useGetAllPdsData(
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
            <SearchBase id="search" label="Search pds" value={searchTerm} placeholder="Search PDS Table"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
            <br />

            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> PDS Table</Card.Header>
                    <Card.Body>
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Pseudo Nhs Number</th>
                                        <th>Org Code</th>
                                        <th>Organisation Name</th>
                                        <th>Relationship With Organisation Effective FromDate</th>
                                        <th>Relationship With Organisation Effective ToDate</th>
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
                                                {pdsRetrieved?.map(
                                                    (pdsDataView: PdsDataView) => (
                                                        <PdsRow
                                                            key={pdsDataView.id}
                                                            pds={pdsDataView}
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

export default PdsTable;