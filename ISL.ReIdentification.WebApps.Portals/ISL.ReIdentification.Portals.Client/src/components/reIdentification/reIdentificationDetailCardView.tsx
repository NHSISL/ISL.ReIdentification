import React, { FunctionComponent, useEffect, useState } from "react";
import { Form, Button, Card, Modal, Spinner, Alert, Tooltip, OverlayTrigger } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import CopyIcon from "../core/copyIcon";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons/faCircleInfo";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { OverlayInjectedProps } from "react-bootstrap/esm/Overlay";

interface Option {
    value: string;
    name: string;
}

interface ReIdentificationDetailCardViewProps {
    lookups: Array<LookupView>;
}

const ReIdentificationDetailCardView: FunctionComponent<ReIdentificationDetailCardViewProps> = (props) => {
    const { lookups } = props;
    const [pseudoCode, setPseudoCode] = useState<string>("");
    const [selectedLookupId, setSelectedLookupId] = useState<string>("");
    const clipboardAvailable = navigator.clipboard;
    const { submit, loading, data } = reIdentificationService.useRequestReIdentification();
    const [submittedPseudoCode, setSubmittedPseudoCode] = useState("");
    const [error, setError] = useState("");
    const account = useMsal();

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const acc = account.accounts[0];
        const identificationRequest: AccessRequest = {
            csvIdentificationRequest: undefined,
            impersonationContext: undefined,
            identificationRequest: {
                id: crypto.randomUUID(),
                identificationItems: [{
                    rowNumber: "1",
                    identifier: pseudoCode,
                    hasAccess: false,
                    message: undefined,
                    isReidentified: undefined,
                }],
                displayName: acc.name || "",
                email: acc.username,
                organisation: "TODO",
                reason: selectedLookupId
            }
        }

        setSubmittedPseudoCode(pseudoCode);
        submit(identificationRequest);
        //setError("");
        //submit(identificationRequest).then((d) => {
        //    submit(d.identificationRequest?.identificationItems[0]);
        //}).catch((error) => {
        //    console.error("Submit failed with error:", error);
        //    setError("Something went wrong.");
        //});
    };

    const handlePseudoCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPseudoCode(e.target.value);
    };

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const lookupOptions: Array<Option> = [
        { value: "", name: "Select Reason..." },
        ...lookups.map((lookup) => ({
            value: lookup.value.toString() || "0",
            name: lookup.value || "",
        })),
    ];

    useEffect(() => {
        console.log(loading)
    },[loading])

    const reset = () => {
        setPseudoCode("");
        setSubmittedPseudoCode("");
        setSelectedLookupId("");
    }

    const renderTooltip = (props: OverlayInjectedProps) => (
        <Tooltip id="info-tooltip" {...props}>
            This page provides a way to upload a single pseudo identifier for reidentification.
        </Tooltip>
    );

    if (!submittedPseudoCode) {
        return (
            <>
                <Card.Title className="text-start">
                    <OverlayTrigger placement="right" overlay={renderTooltip}>
                        <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                    </OverlayTrigger>&nbsp;Reidentify Single Patient
                </Card.Title>

                <Card.Subtitle className="text-start text-muted mb-3">
                    <small>
                        Please paste the pseudo identifer in the box below and
                        provide a reason why you are identifying this patient.
                    </small>
                </Card.Subtitle>
                <Form onSubmit={handleSubmit}>
                    <Form.Group className="text-start">
                        <Form.Label>Pseudo NHS Number:</Form.Label>
                        <Form.Control
                            type="text"
                            name="PseudoCode"
                            value={pseudoCode}
                            maxLength={10}
                            minLength={10}
                            inputMode="numeric"
                            pattern="\d*"
                            onChange={handlePseudoCodeChange}
                            placeholder="Pseudo Number"
                            required />
                    </Form.Group>
                    <Form.Text className="text-muted">
                        <small>Pseudo Numbers need to be 10 characters long and only contain numbers.</small>
                    </Form.Text>
                    <br />
                    <br />
                    <Form.Group className="text-start">
                        <Form.Label>Reidentification Reason:</Form.Label>
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
                    <Button type="submit" disabled={!pseudoCode || !selectedLookupId}>
                        {!loading ? <>Get NHS Number</> : <Spinner />}
                    </Button>
                </Form>
            </>
        );
    }

    if(loading)  {
        return <Spinner />
    }

    const reIdResponse = data.find(x => x.pseudo === submittedPseudoCode);
    if (submittedPseudoCode && reIdResponse) {
        return <>
            <p className="text-start">
                NHS Number:</p>
            <Card bg="success" text="white">
                <Card.Body>
                    {reIdResponse.nhsnumber}&nbsp;
                    {reIdResponse.hasAccess && clipboardAvailable &&
                        <CopyIcon content={reIdResponse.nhsnumber} />
                    }
                    {!reIdResponse.hasAccess && <Modal show={true}>
                        <Modal.Header>
                            <h4>Reidentification not allowed.</h4>
                        </Modal.Header>
                        <Modal.Body>
                            <p>You have tried to reidentify a patient's that our records indicate that you do not have access to.</p>
                            <p>Check that the patient is registered to an GP practice that you have access to.</p>
                            <p>To view your ODS organisations configured in the reidentification tool click <a href="about:blank">here</a> and contact your local ICB should you need further access.</p>
                            <p>Any changes to the patient record regisistration will take 24 hours to apply to the reidentification service </p>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="primary" onClick={reset}>Start Over</Button>
                        </Modal.Footer>
                    </Modal>}
                </Card.Body>
            </Card>
            <br />

            <Button onClick={reset} variant="primary">Start Over</Button>
        </>
    }


    return <>Something went wrong.</>


}

export default ReIdentificationDetailCardView;
