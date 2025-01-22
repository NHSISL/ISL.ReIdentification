import React, { FunctionComponent, useState } from "react";
import { Form, Button, Card, Spinner, Tooltip, OverlayTrigger, Alert } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import { faCircleInfo } from "@fortawesome/free-solid-svg-icons/faCircleInfo";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { OverlayInjectedProps } from "react-bootstrap/esm/Overlay";
import ReidentificationResultView from "./reidentificationResultView";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { getPseudo } from "../../helpers/hxHelpers";

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
    const { submit, loading, data } = reIdentificationService.useRequestReIdentification();
    const [submittedPseudoCode, setSubmittedPseudoCode] = useState("");
    const account = useMsal();

    const { data: userAccessData } = userAccessViewService.useGetAccessForUser(account.accounts[0].idTokenClaims!.oid!);
    const orgCodes = userAccessData?.map(item => item.orgCode);

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
                    identifier: getPseudo(pseudoCode),
                    hasAccess: false,
                    message: undefined,
                    isReidentified: undefined,
                }],
                displayName: acc.name || "",
                email: acc.username,
                organisation: orgCodes?.toString() || "",
                reason: selectedLookupId
            }
        }

        setSubmittedPseudoCode(getPseudo(pseudoCode));
        submit(identificationRequest);
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
                <Card.Header>
                    <Card.Title className="text-start">
                        <OverlayTrigger placement="right" overlay={renderTooltip}>
                            <FontAwesomeIcon icon={faCircleInfo} className="text-primary" size="lg" />
                        </OverlayTrigger>&nbsp;Reidentify Single Patient
                    </Card.Title>
                </Card.Header>
                <Card.Body>
                    <Card.Subtitle className="text-start text-muted mb-3">
                        <small>
                            Please enter the pseudo identifier in the box below and select the
                            reason for identifying the patient from the dropdown menu.
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
                            <Form.Label>Re-identification Reason:</Form.Label>
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
                        <Button type="submit" disabled={!pseudoCode || !selectedLookupId}>
                            {!loading ? <>Get NHS Number</> : <Spinner />}
                        </Button>
                    </Form>
                </Card.Body>
            </>
        );
    }

    if (loading) {
        return <><Card.Header>
            Reidentifying...
        </Card.Header>
            <Card.Body>
                <Spinner />
            </Card.Body>
        </>
    }

    const reIdResponse = data.find(x => x.pseudo === submittedPseudoCode);
    if (submittedPseudoCode && reIdResponse) {
        return <>
            <ReidentificationResultView reidentificationRecord={reIdResponse}>
                <Button onClick={reset} variant="primary">Start Over</Button>
            </ReidentificationResultView>
        </>
    }

    return <>

        <Alert variant="danger" className="mb-0">
            Something went wrong. Please contact <a href="mailto:isl.support@nhs.net">isl.support@nhs.net</a> for support.
        </Alert>
    </>

}

export default ReIdentificationDetailCardView;
