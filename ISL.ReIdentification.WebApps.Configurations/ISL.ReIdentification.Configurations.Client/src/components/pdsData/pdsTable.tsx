import { FunctionComponent, useMemo, useState } from "react";
import { Button, Container, InputGroup, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import PdsRow from "./pdsRow";
import { pdsDataViewService } from "../../services/views/pdsData/pdsDataViewService";
import debounce from "lodash/debounce";
import SearchBase from "../bases/search/SearchBase";
import InfiniteScroll from "../bases/pagers/InfiniteScroll";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { PdsDataView } from "../../models/views/components/pdsData/pdsDataView";
import InfiniteScrollLoader from "../bases/pagers/InfiniteScroll.Loader";
import { faRefresh } from "@fortawesome/free-solid-svg-icons";
import { PdsLoadAudit } from "../audit/pdsLoadAudit";

type PdsTableProps = object;

const PdsTable: FunctionComponent<PdsTableProps> = () => {
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [debouncedTerm, setDebouncedTerm] = useState<string>("");
    const [showSpinner, setShowSpinner] = useState(false);

    const {
        mappedPdsData: pdsRetrieved,
        isLoading,
        fetchNextPage,
        isFetchingNextPage,
        hasNextPage,
        refetch
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

    const handleRefresh = async () => {
        setShowSpinner(true);
        await refetch();
        setShowSpinner(false);
    };

    return (
        <>
            <PdsLoadAudit isAlert={true} />
            <InputGroup className="mb-3">
            <SearchBase id="search" label="Search pds" value={searchTerm} placeholder="Search PDS Table"
                onChange={(e) => { handleSearchChange(e.currentTarget.value) }} />
                <Button variant="outline-secondary" onClick={handleRefresh}>
                    <FontAwesomeIcon icon={faRefresh} />
                </Button>
            </InputGroup>

            <Container fluid className="infiniteScrollContainer">
               
                        <InfiniteScroll loading={isLoading || showSpinner} hasNextPage={hasNextPage || false} loadMore={fetchNextPage}>

                            <Table striped bordered hover variant="light" responsive>
                                <thead>
                                    <tr>
                                        <th>Pseudo Identifier</th>
                                        <th>Organisation Code</th>
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
            </Container>
        </>
    );
};

export default PdsTable;