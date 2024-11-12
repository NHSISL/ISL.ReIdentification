import { FunctionComponent, useState, useEffect } from "react";
import { Alert, Button, Card, Col, Container, Form, OverlayTrigger, Row, Spinner, Tooltip } from "react-bootstrap";
import { csvIdentificationRequestService } from "../../services/foundations/csvIdentificationRequestService";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { OverlayInjectedProps } from "react-bootstrap/esm/Overlay";

interface CsvReIdentificationDownloadDetailProps {
    csvIdentificationRequestId: string | undefined;
}

interface Option {
    value: string;
    name: string;
}

const CsvReIdentificationDownloadDetail: FunctionComponent<CsvReIdentificationDownloadDetailProps> = ({ csvIdentificationRequestId }) => {

    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");

    const {
        data,
        error
    } = csvIdentificationRequestService
        .useSelectCsvIdentificationByCsvIdentificationRequestIdRequest(
            csvIdentificationRequestId!);

    const { fetch, loading: fetchLoading, data: fetchData, filename, error: fetchError }
        = reIdentificationService.useGetCsvIdentificationRequestById(csvIdentificationRequestId!, selectedLookupId);

    useEffect(() => {
        if (fetchData) {
            const csvContent = typeof fetchData === 'string' ? fetchData : JSON.stringify(fetchData);
            const blob = new Blob([csvContent], { type: 'text/csv' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename; // Use the filename from the hook
            document.body.appendChild(a);
            a.click();
            a.remove();
        }
    }, [fetchData, filename]);

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

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        try {
            await fetch();
        } catch (error) {
            console.error("Error downloading the file", error);
        }
    }

    const renderTooltip = (props: OverlayInjectedProps) => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to download the reidentified CSV.
        </Tooltip>
    );

    return (
        <Container fluid>
            <Row className="justify-content-md-center mt-3">
                <Card style={{ width: '50rem' }}>
                    <Card.Body>
                        <Card.Title className="text-start">
                            <OverlayTrigger placement="right" overlay={renderTooltip}>
                                <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                            </OverlayTrigger>&nbsp;Download Reidentified CSV
                        </Card.Title>

                        <Alert variant="info">
                            <Row>
                                <Col md={6} className="mb-3">
                                    <div><strong>Requester Display Name:</strong> <span>{data.requesterDisplayName}</span></div>
                                    <div><strong>Requester Email:</strong> <span>{data.requesterEmail}</span></div>
                                    <div><strong>Recipient Display Name:</strong> <span>{data.recipientDisplayName}</span></div>
                                    <div><strong>Recipient Email:</strong> <span>{data.recipientEmail}</span></div>
                                </Col>
                                <Col md={6} className="mb-3">
                                    <div><strong>Reason:</strong> <span>{data.reason}</span></div>
                                    <div><strong>Organisation:</strong> <span>{data.organisation}</span></div>
                                    <div><strong>Identifier Column:</strong> <span>{data.identifierColumnIndex}</span></div>
                                    <div><strong>FileName:</strong> <span>{data.filepath}</span></div>
                                </Col>

                            </Row>
                        </Alert>

                        <Form onSubmit={handleSubmit}>
                            <Form.Group className="text-start">
                                <Form.Label><strong>Reidentification reason:</strong></Form.Label>
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
                                <Form.Text className="text-muted">
                                    Please supply a reason why you are requesting to Reidentify this csv of patients.
                                </Form.Text>
                            </Form.Group>
                            <br />
                            {fetchError && <Alert variant="danger">
                                Error: {fetchError.message}
                            </Alert>}
                            <Button type="submit" disabled={!selectedLookupId || fetchLoading}>
                                {!fetchLoading ? <>Download</> : <Spinner />}
                            </Button>
                            <Card.Subtitle className="text-start mb-3 mt-3">
                                <small>
                                    On <strong>Download</strong>, the file will be saved to your local machine. Please ensure you check your downloads folder.
                                </small>
                            </Card.Subtitle>
                        </Form>
                    </Card.Body>
                </Card>
            </Row>
        </Container>
    );
};

export default CsvReIdentificationDownloadDetail;