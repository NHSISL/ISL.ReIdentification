import { FunctionComponent, useEffect, useState } from "react"
import { Button, Form, FormCheck, FormGroup, Spinner } from "react-bootstrap"
import { reIdentificationService } from "../../services/foundations/reIdentificationService"
import { useMsal } from "@azure/msal-react"
import { AccessRequest } from "../../models/accessRequest/accessRequest"
import ReidentificationResultView from "./reidentificationResultView"
import { lookupViewService } from "../../services/views/lookups/lookupViewService"

type ReidentificationWebPartProps = {
    pseudo?: string
}

const ReidentificationWebPart: FunctionComponent<ReidentificationWebPartProps> = ({ pseudo }) => {

    const { submit, loading, data } = reIdentificationService.useRequestReIdentification();
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("");
    const [rememberChecked, setRememberChecked] = useState(false);
    const [reidReason, setReidReason] = useState("");
    const [reasonForgot, setReasoneForgot] = useState(false);
    const account = useMsal();

    const handleLookupChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setReidReason(e.target.value);
    };

    const rememberReason = () => {
        localStorage[`reason:${account.accounts[0].username}`] = reidReason;
        setReasoneForgot(false)
    }

    const forgetReason = () => {
        localStorage.removeItem(`reason:${account.accounts[0].username}`);
        setReasoneForgot(true)
    }

    const reid = (reason: string) => {

        if (rememberChecked) {
            rememberReason();
        }

        if (pseudo) {
            const acc = account.accounts[0];

            const identificationRequest: AccessRequest = {
                csvIdentificationRequest: undefined,
                impersonationContext: undefined,
                identificationRequest: {
                    id: crypto.randomUUID(),
                    identificationItems: [{
                        rowNumber: "1",
                        identifier: pseudo,
                        hasAccess: false,
                        message: undefined,
                        isReidentified: undefined,
                    }],
                    displayName: acc.name || "",
                    email: acc.username,
                    organisation: "TODO",
                    reason: reason
                }
            }

            submit(identificationRequest)
        }
    }

    useEffect(() => {
        const reason = localStorage[`reason:${account.accounts[0].username}`];
        if (pseudo && reason) {
            setRememberChecked(true)
            setReidReason(reason);
            reid(reason);
        }
    }, [])


    if (!pseudo) {
        return <div>Provide Id.</div>
    }

    if (!reidReason) {
        <Form.Select
            value={reidReason}
            onChange={handleLookupChange}
            required >
            {mappedLookups.map((option) => (
                <option key={option.value} value={option.value}>
                    {option.name}
                </option>
            ))}
        </Form.Select>
    }

    if (loading) {
        return <Spinner />
    }

    if (data && reidReason) {
        const record = data.find(data => data.pseudo === pseudo);
        if (record) {
            return <ReidentificationResultView reidentificationRecord={record}>
                <>
                    <p>Reidentification Reason: {reidReason}</p>
                    {rememberChecked ? <>
                            { reasonForgot ? 
                                <p>Reidentification Reason Forgotten, you will be asked to select a new reason on the next reidentification request.</p> 
                                : 
                                <Button onClick={forgetReason} size="sm">Change Reason</Button>}
                        </>
                        :
                        <Button onClick={rememberReason} size="sm">Remember Reason</Button>
                    }
                </>
            </ReidentificationResultView>
        }
    }

    return <>
        <Form>
            {isLoading ? <Spinner /> : <>
                <Form.Group >
                    <Form.Label>Select Reason for Re-identification</Form.Label>
                    <Form.Select
                        value={reidReason}
                        onChange={handleLookupChange}
                        required >
                        <option value={""}>--- Please Select ---</option>
                        {mappedLookups.map((option) => (
                            <option key={option.value} value={option.value}>
                                {option.name}
                            </option>
                        ))}
                    </Form.Select>
                </Form.Group>
                <FormGroup className="mb-3">
                    <FormCheck label="Remember Reason" onChange={(e) => { setRememberChecked(e.target.checked) }} />
                </FormGroup>
                <div className="d-grid gap-2">
                    <Button onClick={() => { reid(reidReason) }} disabled={!reidReason}>Re-identify</Button>
                </div>
            </>
            }
        </Form>
    </>
}

export default ReidentificationWebPart;