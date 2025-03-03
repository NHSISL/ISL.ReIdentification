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
import CopyIcon from "../core/copyIcon";

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

    const updateImpersonation = reIdentificationService.useRequestReIdentificationImpersonationApproval();

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

            await updateImpersonation
                .submitApproval(impersonationIdentificationRequestId!, isApproved);

            setAccessRequest(null);
            setSuccess("");
        } catch (error) {
            setErrorStatus(error + " .Something went wrong when denying the request, please contact an administrator.");
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
                                        <Button variant="success" onClick={() => handleDeny(true)}>
                                            {!isDenying ? "Approve Token Generation" : <Spinner animation="border" size="sm" />}
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
                                            <Button type="submit" onClick={handleReGenerateClick} variant="success">
                                                {!loading ? "Generate Tokens" : <Spinner animation="border" size="sm" />}
                                            </Button>
                                        </ButtonGroup>
                                    )
                                )}

                                {!data?.isApproved && !error && (
                                    <Alert variant="warning" className="mt-3">
                                        <p>
                                            This project has not been approved by the responsible person, <strong>{data?.responsiblePersonDisplayName}</strong>.
                                        </p>
                                        <p>
                                            As a result, you will not be able to generate tokens or proceed with any actions that require approval.
                                            Please contact the responsible person to discuss the approval status of this project.
                                        </p>
                                        <p>
                                            If you believe this is an error, ensure that the responsible person, <strong>{data?.responsiblePersonDisplayName}</strong>,
                                            has reviewed and approved the project request. You can reach out to them via email at <strong>{data?.responsiblePersonEmail}</strong>.
                                        </p>
                                    </Alert>
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
                                                    <>
                                                        <Alert variant="info">
                                                            NOTE: These tokens will not be available to copy again after refreshing or closing this page.
                                                        </Alert>
                                                        <Table striped bordered hover>
                                                            <thead>
                                                                <tr>
                                                                    <th><small>Token Type</small></th>
                                                                    <th><small>Token</small></th>
                                                                    <th><small>Copy</small></th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr>
                                                                    <td><small><strong>Errors SAS Token:</strong></small></td>
                                                                    <td><small>{accessRequest.impersonationContext.errorsSasToken}</small></td>
                                                                    <td className="text-center">
                                                                        <CopyIcon
                                                                            content={accessRequest.impersonationContext.errorsSasToken || ""}
                                                                            resetTime={2000} />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td><small><strong>Inbox SAS Token:</strong></small></td>
                                                                    <td><small>{accessRequest.impersonationContext.inboxSasToken}</small></td>
                                                                    <td className="text-center">
                                                                        <CopyIcon
                                                                            content={accessRequest.impersonationContext.inboxSasToken || ""}
                                                                            resetTime={2000} />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td><small><strong>Outbox SAS Token:</strong></small></td>
                                                                    <td><small>{accessRequest.impersonationContext.outboxSasToken}</small></td>
                                                                    <td className="text-center"s>
                                                                        <CopyIcon
                                                                            content={accessRequest.impersonationContext.outboxSasToken || ""}
                                                                            resetTime={2000} />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </Table>
                                                    </>
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