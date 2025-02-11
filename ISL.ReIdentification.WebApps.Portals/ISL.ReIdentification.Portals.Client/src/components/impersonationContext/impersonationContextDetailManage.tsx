import { FunctionComponent, useState, useEffect } from "react";
import Container from "react-bootstrap/esm/Container";
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase";
import { Alert, Button, ButtonGroup, Card, Col, ListGroup, Row, Spinner, Table } from "react-bootstrap";
import { impersonationContextService } from "../../services/foundations/impersonationContextService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheck, faTimes } from "@fortawesome/free-solid-svg-icons";
import { useMsal } from "@azure/msal-react";
import { impersonationContextViewService } from "../../services/views/impersonationContext/impersonationContextViewService";
import { ImpersonationContext } from "../../models/impersonationContext/impersonationContext";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";

interface ImpersonationContextDetailManageProps {
    impersonationIdentificationRequestId: string | undefined;
}

const ImpersonationContextDetailManage: FunctionComponent<ImpersonationContextDetailManageProps> = ({ impersonationIdentificationRequestId }) => {

    const account = useMsal();

    const { data, error, refetch } = impersonationContextService.useRetrieveAllImpersonationById(impersonationIdentificationRequestId!);

    const { submit, loading } = reIdentificationService.useRequestReIdentificationImpersonationGenerateTokens();
    const [errorStatus, setErrorStatus] = useState("");
    const [success, setSuccess] = useState("");
    const [accessRequest, setAccessRequest] = useState<any>(null);
    const [confirmReGenerate, setConfirmReGenerate] = useState(false);
    const [isRegenerating, setIsRegenerating] = useState(false);
    const [isDenying, setIsDenying] = useState(false);

    const updateImpersonation = impersonationContextViewService.useUpdateImpersonationContext();

    useEffect(() => {
        if (impersonationIdentificationRequestId) {
            refetch();
        }
    }, [impersonationIdentificationRequestId, accessRequest, refetch]);

    const generateTokens = async (impersonationIdentificationRequestId: string | undefined) => {
        setIsRegenerating(true);
        return submit(impersonationIdentificationRequestId!).then((response) => {
            setSuccess("Generated Tokens successfully! Please copy your tokens below.");
            setAccessRequest(response);
            setConfirmReGenerate(false);
            setIsRegenerating(false);
        }).catch(() => {
            setErrorStatus("Something went wrong when generating, please contact an administrator.");
            setIsRegenerating(false);
        });
    };

    const handleDeny = async (isApproved: boolean) => {
        setIsDenying(true);
        try {
            setAccessRequest(null);
            await generateTokens(impersonationIdentificationRequestId);

            const updatedImpersonationContext: ImpersonationContext = {
                ...data!,
                isApproved: isApproved,
            };

            await updateImpersonation.mutateAsync(updatedImpersonationContext);

            setAccessRequest(null);
            setSuccess("");
        } catch (error) {
            setErrorStatus(error + "Something went wrong when denying the request, please contact an administrator.");
        } finally {
            setIsDenying(false);
        }
    };

    const handleSubmit = (e: React.FormEvent<HTMLButtonElement>, impersonationIdentificationRequestId: string | undefined) => {
        e.preventDefault();
        generateTokens(impersonationIdentificationRequestId);
    };

    const handleReGenerateClick = () => {
        setConfirmReGenerate(true);
    };

    const handleCancelReGenerate = () => {
        setConfirmReGenerate(false);
    };

    const copyToClipboard = (text: string) => {
        navigator.clipboard.writeText(text);
    };

    return (
        <Container fluid>
            <section>
                <BreadCrumbBase
                    link="/project"
                    backLink="Projects"
                    currentLink="Manage Project">
                </BreadCrumbBase>
            </section>

            <Row className="justify-content-md-center mt-3">
                <Card style={{ width: '70rem' }}>
                    <Card.Body>
                        <Card.Title className="text-start">
                            Manage Project ({data?.projectName})
                        </Card.Title>

                        <Alert variant="info">
                            <Row>
                                <Col md={6} className="mb-3">
                                    <div><strong>Requester Display Name:</strong> <span>{data?.requesterDisplayName}</span></div>
                                    <div><strong>Requester Email:</strong> <span>{data?.requesterEmail}</span></div>
                                    <div><strong>Responsible Person Display Name:</strong> <span>{data?.responsiblePersonDisplayName}</span></div>
                                    <div><strong>Responsible Person Email:</strong> <span>{data?.responsiblePersonEmail}</span></div>
                                </Col>
                                <Col md={6} className="mb-3">
                                    <div><strong>Reason:</strong> <span>{data?.reason}</span></div>
                                    <div><strong>Approved: </strong><span>
                                        {data?.isApproved
                                            ? <FontAwesomeIcon icon={faCheck} className="text-success" />
                                            : <FontAwesomeIcon icon={faTimes} className="text-danger" />}
                                    </span></div>
                                </Col>
                            </Row>
                        </Alert>

                        {account.accounts[0].idTokenClaims?.oid?.toLowerCase() === data?.responsiblePersonEntraUserId.toLowerCase() && (
                            <>
                                {data?.isApproved ? (
                                    <div className="mb-3">
                                        <p>As the responsible person for this project, you have the authority to deny access. Note: Doing so will revoke all previously granted permissions, requiring new keys to be generated.</p>
                                        <Button variant="danger" onClick={() => handleDeny(false)}>
                                            {!isDenying ? "Deny Token Generation" : <Spinner animation="border" size="sm" />}
                                        </Button>
                                    </div>
                                ) : (
                                    <div className="mb-3">
                                        <p>You have been designated as the responsible person for this project. To enable the service to process files dropped into it, please confirm that this request is correct.</p>
                                        <Button type="submit" variant="success" onClick={(e) => handleSubmit(e, impersonationIdentificationRequestId)}>
                                            {!loading ? "Approve And Generate Tokens" : <Spinner animation="border" size="sm" />}
                                        </Button>
                                    </div>
                                )}
                            </>
                        )}

                        {account.accounts[0].idTokenClaims?.oid === data?.requesterEntraUserId && (
                            <>
                                {data?.isApproved && !error && (
                                    confirmReGenerate ? (
                                        <div className="mb-3">
                                            <p>If you continue, you will expire any existing previous tokens. Do you want to proceed?</p>
                                            <ButtonGroup>
                                                <Button variant="danger" onClick={handleCancelReGenerate}>Cancel</Button>
                                                <Button variant="success" onClick={(e) => handleSubmit(e, impersonationIdentificationRequestId)}>
                                                    {!isRegenerating ? "OK" : <Spinner animation="border" size="sm" />}
                                                </Button>
                                            </ButtonGroup>
                                        </div>
                                    ) : (
                                        <ButtonGroup className="mb-3">
                                            <Button type="submit" onClick={handleReGenerateClick}>
                                                {!loading ? "Re-Generate Tokens" : <Spinner animation="border" size="sm" />}
                                            </Button>
                                        </ButtonGroup>
                                    )
                                )}
                                {errorStatus && <Alert variant="danger" className="mt-2">{errorStatus}</Alert>}

                                {accessRequest && (
                                    <>
                                        {success && <Alert variant="success" className="mt-2">{success}</Alert>}
                                        <Card className="mt-4">
                                            <Card.Header>Token Generation</Card.Header>
                                            <Card.Body>
                                                {isRegenerating ? (
                                                    <Spinner animation="border" size="lg" />
                                                ) : (
                                                    <Table striped bordered hover>
                                                        <thead>
                                                            <tr>
                                                                <th>Token Type</th>
                                                                <th>Token</th>
                                                                <th>Action</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <tr>
                                                                <td><strong>Errors SAS Token:</strong></td>
                                                                <td><small>{accessRequest.impersonationContext.errorsSasToken}</small></td>
                                                                <td>
                                                                    <Button variant="outline-primary" size="sm" onClick={() => copyToClipboard(accessRequest.impersonationContext.errorsSasToken)}>Copy</Button>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><strong>Inbox SAS Token:</strong></td>
                                                                <td><small>{accessRequest.impersonationContext.inboxSasToken}</small></td>
                                                                <td>
                                                                    <Button variant="outline-primary" size="sm" onClick={() => copyToClipboard(accessRequest.impersonationContext.inboxSasToken)}>Copy</Button>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><strong>Outbox SAS Token:</strong></td>
                                                                <td><small>{accessRequest.impersonationContext.outboxSasToken}</small></td>
                                                                <td>
                                                                    <Button variant="outline-primary" size="sm" onClick={() => copyToClipboard(accessRequest.impersonationContext.outboxSasToken)}>Copy</Button>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </Table>
                                                )}
                                            </Card.Body>
                                        </Card>
                                    </>
                                )}
                            </>
                        )}
                    </Card.Body>
                </Card>
            </Row>
        </Container>
    );
};

export default ImpersonationContextDetailManage;