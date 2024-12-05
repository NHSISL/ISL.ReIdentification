import debounce from "lodash/debounce";
import { FunctionComponent, useMemo, useState } from "react";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { Card, Container, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDatabase } from "@fortawesome/free-solid-svg-icons";
import { CsvIdentificationRequestView } from "../../models/views/components/csvIdentificationRequest/csvIdentificationRequestView";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { csvIdentificationRequestViewService } from "../../services/views/csvIdentificationRequest/csvIdentificationRequestViewService";
import SearchBase from "../bases/inputs/SearchBase";
import CsvIdentificationRequestRow from "./csvIdentificationRequestRow";

type CsvIdentificationRequestTableProps = object;

const CsvIdentificationRequestTable: FunctionComponent<CsvIdentificationRequestTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner] = useState(false);


    const {
        mappedCsvIdentificationRequests: csvIdentificationRequestRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
    } = csvIdentificationRequestViewService.useGetAllCsvIdentificationRequests(
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
        return true;
        //return !isLoading && data?.pages.at(-1)?.nextPage === undefined;
    };

    return (
        <>
            <SearchBase id="search" label="Search CSV Requests" value={searchTerm} placeholder="Search Csv Identitfation Requests"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
            <br />

            <Container fluid className="infiniteScrollContainer">
                <Card>
                    <Card.Header> <FontAwesomeIcon icon={faDatabase} className="me-2" /> Csv Identification Reques</Card.Header>
                    <Card.Body>
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light">
                                <thead>
                                    <tr>
                                        <th>Requester FirstName</th>
                                        <th>Requester LastName</th>
                                        <th>Requester Email</th>
                                        <th>Recipient FirstName</th>
                                        <th>Recipient LastName</th>
                                        <th>Reason</th>
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
                                            {csvIdentificationRequestRetrieved?.map(
                                                (csvIdentificationRequestView: CsvIdentificationRequestView) => (
                                                    <CsvIdentificationRequestRow
                                                        key={csvIdentificationRequestView.id.toString()}
                                                        csvIdentificationRequest={csvIdentificationRequestView}
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

export default CsvIdentificationRequestTable;