import { FunctionComponent } from "react";
import Container from "react-bootstrap/esm/Container";
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase";
import { Alert, Button, Card, Col, Row } from "react-bootstrap";
import { impersonationContextService } from "../../services/foundations/impersonationContextService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCheck, faTimes } from "@fortawesome/free-solid-svg-icons";
import { useMsal } from "@azure/msal-react";

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
                                    <div><strong>Organisation:</strong> <span>{data?.organisation}</span></div>
                                    <div><strong>Identifier Column:</strong> <span>{data?.identifierColumn}</span></div>
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
                                {data?.isApproved ? <Button variant="danger">Deny</Button> : <Button variant="success">Approve</Button>}
                            </>
                        )}

                        <br /><br />
                        {account.accounts[0].idTokenClaims?.oid === data?.requesterEntraUserId && (
                            <>
                                <p>As the Owner of this project you have the ability to regenrate the SAS Tokens for your container, once clicked the old token will be invalid and you will have to setup the new tokens to ensure files are processed.</p>
                                <Button variant="dark">Regenerate Tokens</Button>&nbsp;
                            </>
                        )} &nbsp;

                    </Card.Body>
                </Card>
            </Row>

        </>
    )
};

export default ImpersonationContextDetailManage;