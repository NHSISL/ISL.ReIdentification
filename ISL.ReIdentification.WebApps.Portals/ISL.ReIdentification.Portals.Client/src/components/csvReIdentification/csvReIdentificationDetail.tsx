import { FunctionComponent } from "react";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { Container, Row, Spinner } from "react-bootstrap";
import CsvReIdentificationDetailCard from "./csvReIdentificationDetailCard";

const CsvReIdentificationDetail: FunctionComponent = () => {
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("");

    if (isLoading) {
        return <Spinner />
    }

    return (
        <>
            <div>
                <p>This page provides a way to upload a CSV of pseudo identifiers for reidentification, please also select the column used for the pseudo identifier.</p>
                <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation, 000000000000 will be returned if not found or no access found.</p>
                <p><strong>Note:</strong> all reidentification requests are subject to breach monitoring and reporting</p>
                <p>Details of breach thresholds can be found <a href="about:blank" target="blank" >here</a></p>
            </div>
            {mappedLookups &&
                <Container className="text-center">
                    <Row className="justify-content-md-center">
                        <CsvReIdentificationDetailCard
                            lookups={mappedLookups}
                        />
                    </Row>
                </Container>
            }
        </>
    );
};

export default CsvReIdentificationDetail;