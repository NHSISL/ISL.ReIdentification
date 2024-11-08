import React, { FunctionComponent, useState } from "react";
import { Form, Button, Card, Spinner } from "react-bootstrap";
import { LookupView } from "../../models/views/components/lookups/lookupView";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";
import ReidentificationResultView from "./reidentificationResultView";

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
    };

    const handlePseudoCodeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPseudoCode(e.target.value);
    };

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedLookupId(e.target.value);
    };

    const lookupOptions: Array<Option> = [
        { value: "", name: "Select Purpose..." },
        ...lookups.map((lookup) => ({
            value: lookup.value.toString() || "0",
            name: lookup.name || "",
        })),
    ];

    const reset = () => {
        setPseudoCode("");
        setSubmittedPseudoCode("");
        setSelectedLookupId("");
    }

    if (!submittedPseudoCode) {
        return (
            <>
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
                            onChange={handlePseudoCodeChange}
                            placeholder="Pseudo Number"
                            required />
                    </Form.Group>
                    <br />
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
                    <Button type="submit" disabled={!pseudoCode || !selectedLookupId}>
                        {!loading ? <>Get NHS Number</> : <Spinner />}
                    </Button>
                </Form>
            </>
        );
    }

    if (loading) {
        return <Spinner />
    }

    const reIdResponse = data.find(x => x.pseudo === submittedPseudoCode);
    if (submittedPseudoCode && reIdResponse) {
        return <>

            <ReidentificationResultView reidentificationRecord={reIdResponse}>
                <Button onClick={reset} variant="primary">Start Over</Button>
            </ReidentificationResultView>


        </>
    }


    return <>Something went wrong.</>


}

export default ReIdentificationDetailCardView;
