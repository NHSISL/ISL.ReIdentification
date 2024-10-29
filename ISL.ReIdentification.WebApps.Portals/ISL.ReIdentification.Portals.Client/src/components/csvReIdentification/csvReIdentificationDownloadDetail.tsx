import { FunctionComponent, useState } from "react";
import { Alert, Button, Col, Container, Form, Row, Spinner } from "react-bootstrap";
import { csvIdentificationRequestService } from "../../services/foundations/csvIdentificationRequestService";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";

interface CsvReIdentificationDownloadDetailProps {
    csvIdentificationRequestId: string | undefined;
}

interface Option {
    value: string;
    name: string;
}


const CsvReIdentificationDownloadDetail: FunctionComponent<CsvReIdentificationDownloadDetailProps> = ({ csvIdentificationRequestId }) => {

    const { mappedLookups, loading } = lookupViewService.useGetAllLookups("");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");

    const {
        data,
        error,
        isLoading
    } = csvIdentificationRequestService
        .useSelectCsvIdentificationByCsvIdentificationRequestIdRequest(
            csvIdentificationRequestId!);

    if (isLoading) {
        return <p><Spinner /></p>;
    }

    if (error) {
        return <p>Error: {error.message}</p>;
    }

    if (!data) {
        return <p>No data available</p>;
    }

    const lookupOptions: Array<Option> = [
        { value: "", name: "Select Purpose..." },
        ...mappedLookups.map((lookup) => ({
            value: lookup.value.toString() || "0",
            name: lookup.name || "",
        })),
    ];

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
    }


    return (
        <Container fluid>
            <Row className="justify-content-md-center">
                <h2>Data Set ReIdentification</h2>

                <Alert variant="info">
                    <Row>
                        <Col md={4} className="mb-3">
                            <div><strong>Requester Entra User ID:</strong> <span>{data.requesterEntraUserId}</span></div>
                            <div><strong>Requester First Name:</strong> <span>{data.requesterFirstName}</span></div>
                            <div><strong>Requester Last Name:</strong> <span>{data.requesterLastName}</span></div>
                            <div><strong>Requester Display Name:</strong> <span>{data.requesterDisplayName}</span></div>
                            <div><strong>Requester Email:</strong> <span>{data.requesterEmail}</span></div>
                            <div><strong>Requester Job Title:</strong> <span>{data.requesterJobTitle}</span></div>
                        </Col>
                        <Col md={4} className="mb-3">

                            <div><strong>Recipient Entra User ID:</strong> <span>{data.recipientEntraUserId}</span></div>
                            <div><strong>Recipient First Name:</strong> <span>{data.recipientFirstName}</span></div>
                            <div><strong>Recipient Last Name:</strong> <span>{data.recipientLastName}</span></div>
                            <div><strong>Recipient Display Name:</strong> <span>{data.recipientDisplayName}</span></div>
                            <div><strong>Recipient Email:</strong> <span>{data.recipientEmail}</span></div>
                            <div><strong>Recipient Job Title:</strong> <span>{data.recipientJobTitle}</span></div>
                        </Col>
                        <Col md={4} className="mb-3">

                            <div><strong>Reason:</strong> <span>{data.reason}</span></div>
                            <div><strong>Organisation:</strong> <span>{data.organisation}</span></div>
                            <div><strong>SHA256 Hash:</strong> <span>{data.sha256Hash}</span></div>
                            <div><strong>Identifier Column:</strong> <span>{data.identifierColumn}</span></div>
                        </Col>
                    </Row>
                </Alert>

                <Row>
                    <p>Hello, in order to download this file, please specify the reason for using this data?</p>

                    <Form onSubmit={handleSubmit}>
                        <Form.Group className="text-start">
                            <Form.Label>Reidentification reason:</Form.Label>
                            <Form.Select
                                value={selectedLookupId}
                                onChange={handleLookupChange}
                                required >
                                {lookupOptions.map((option) => (
                                    <option key={option.value} value={option.value}>
                                        {option.name}
                                    </option>
                                ))}
                            </Form.Select>
                        </Form.Group>
                        <br />
                        {error && <Alert variant="danger">
                            Something went Wrong.
                        </Alert>}
                        <Button type="submit" disabled={!selectedLookupId}>
                            {!loading ? <>Download</> : <Spinner />}
                        </Button>
                    </Form>

                       TODO:  Implement Download link
               
                </Row>
            </Row>
        </Container>
    );
};

export default CsvReIdentificationDownloadDetail;