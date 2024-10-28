import { FunctionComponent } from "react";
import ReIdentificationDetailCard from "./reIdentificationDetailCard";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { Container, Row, Spinner } from "react-bootstrap";

const ReIdentificationDetail: FunctionComponent = () => {
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("");

    if (isLoading) {
        return <Spinner />
    }

    return (
        <>
            <div>
                <h2>Welcome to the re-identifcation Portal</h2>
                <p>This page provides a simple reidentification for a single patient pseudo identifer</p>
                <p>To do this please paste the pseudo identifer in the box below and provide a reason why you are identifying this patient.</p>
                <p>Note: you will only be able to reidentify patients that are present within your organisation.</p>
                <p>Note: all reidentification requests are subject to breach monitoring and reporting</p>
                <p>Details of breach thresholds can be found <a href="about:blank" target="blank" >here</a></p>
            </div>
            {mappedLookups &&
                <Container className="text-center">
                    <Row className="justify-content-md-center">
                        <ReIdentificationDetailCard
                            lookups={mappedLookups}
                        />
                    </Row>
                </Container>
            }
        </>
    );
};

export default ReIdentificationDetail;