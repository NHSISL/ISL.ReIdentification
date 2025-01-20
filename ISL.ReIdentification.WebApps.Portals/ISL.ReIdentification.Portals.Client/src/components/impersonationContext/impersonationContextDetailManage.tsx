import { FunctionComponent, useState } from "react";
import Container from "react-bootstrap/esm/Container";
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase";
import { Alert, Button, Card, Col, Row, Spinner } from "react-bootstrap";
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

    const {
        data,
        error
    } = impersonationContextService
        .useRetrieveAllImpersonationById(
            impersonationIdentificationRequestId!);

    const { submit, loading } = reIdentificationService.useRequestReIdentificationImpersonationGenerateTokens();
    const [ errorStatus, setErrorStatus] = useState("");
    const [ success, setSuccess] = useState("");

    const updateImpersonation = impersonationContextViewService.useUpdateImpersonationContext();
    const handleUpdate = (isApproved: boolean) => {
        const updatedImpersonationContext: ImpersonationContext = {
            ...data,
            isApproved: isApproved,
        };
        return updateImpersonation.mutateAsync(updatedImpersonationContext);
    };

    const handleSubmit = (e: React.FormEvent<HTMLButtonElement>, impersonationIdentificationRequestId: string | undefined) => {
        e.preventDefault();
        alert(impersonationIdentificationRequestId)
        submit(impersonationIdentificationRequestId!).then((message) => {
            setSuccess("generated Tokens succesfully " + message);
        }).catch(() => {
            setErrorStatus("Something went wrong when generating, please contact an administrator.");
        })
    };

    return (
        <>
            <Container fluid>

                <section>
                    <BreadCrumbBase
                        link="/project"
                        backLink="Projects"
                        currentLink="Manage Project">
                    </BreadCrumbBase>

                </section>
            </Container>

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
                                    <div><strong>Requester Display Name:</strong> <span>{data?.requesterEmail}</span></div>
                                    <div><strong>Requester Display Name:</strong> <span>{data?.responsiblePersonDisplayName}</span></div>
                                    <div><strong>Requester Display Name:</strong> <span>{data?.responsiblePersonEmail}</span></div>

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

                        {/*<strong>Logged in user: </strong>{account.accounts[0].idTokenClaims?.oid} <br />*/}
                        {/*<strong>Requester ID: </strong>{data?.requesterEntraUserId}  <br />*/}
                        {/*<strong>Respnosible Person ID: </strong>{data?.responsiblePersonEntraUserId}<br /><br />*/}

                        {/* TODO: Approve Deny update */}
                        {/* TODO: Regenerate Call */}


                        {account.accounts[0].idTokenClaims?.oid === data?.responsiblePersonEntraUserId && (
                            <>
                                <p>You have been selected as the responsible owner for this project, in order for the service to process files dropped to this service please accept that this is correct.</p>
                                {data?.isApproved ?
                                    <Button variant="danger" onClick={() => handleUpdate(false)}>Deny</Button>
                                    :
                                    <Button variant="success" onClick={() => handleUpdate(true)}>Approve</Button>
                                }
                            </>
                        )}

                        <br /><br />
                        {account.accounts[0].idTokenClaims?.oid === data?.requesterEntraUserId && (
                            <>
                                <p>As the Owner of this project you have the ability to generate the SAS Tokens for your container. Once clicked, the old token will be invalid and you will have to set up the new tokens to ensure files are processed.</p>

                                <Button type="submit" disabled={!data?.isApproved || !!error} onClick={(e) => handleSubmit(e, impersonationIdentificationRequestId)}>
                                    {!loading ? "Generate Tokens" : <Spinner />}
                                </Button>
                                <br /><br />
                                {errorStatus && <Alert variant="danger">
                                    {errorStatus}
                                </Alert>}

                                {success && <Alert variant="success">
                                    {success}
                                </Alert>}

                            </>
                        )} &nbsp;

                    </Card.Body>
                </Card>
            </Row>

        </>
    )
};

export default ImpersonationContextDetailManage;