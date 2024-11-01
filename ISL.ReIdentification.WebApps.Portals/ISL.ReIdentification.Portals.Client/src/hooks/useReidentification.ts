import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../models/accessRequest/accessRequest";
import { useEffect, useState } from "react";
import { ReIdRecord } from "../types/ReIdRecord";
import { reIdentificationService } from "../services/foundations/reIdentificationService";

type TrackedAccessRequest = {
    called: boolean,
    completed: boolean,
    accessRequest: AccessRequest
}

export function useReidentification(reason: string) {
    const [reidentifications, setReidentifications] = useState<ReIdRecord[]>([]);
    const [identificationRequests, setIdentificationRequests] = useState<TrackedAccessRequest[]>([]);
    const [lastPseudo, setLastPseudo] = useState("");
    const { accounts } = useMsal();
    const { submit } = reIdentificationService.useRequestReIdentification();

    function isHx(hxNumber: string) {
        return false;
    }

    function convertHx(hxNumber: string) {
        return hxNumber;
    }

    useEffect(() => {
        async function execute() {

            // process access requests to call
            const reidentificationsToCall = identificationRequests.filter(x => !x.called)
            const next = [...identificationRequests];

            if (reidentificationsToCall) {
                reidentificationsToCall.forEach(async x => {
                    const pseudoNumber = x.accessRequest.identificationRequest?.identificationItems[0].identifier;

                    if(reidentifications.filter(x => x.pseudo === pseudoNumber).length) {
                        // number has already been re-id'd or is in the proces of.
                        return;
                    }


                    x.called = true;
                    setIdentificationRequests(next);

                    setReidentifications((reidentifications) => {
                        const next = [...reidentifications];
                        next.push({
                            pseudo: x.accessRequest.identificationRequest?.identificationItems[0].identifier as string,
                            loading: true, hasCalled: true
                        }
                        );
                        return next;
                    });
                    const result = await submit(x.accessRequest);
                    x.completed = true;

                    setReidentifications((reidentifications) => {
                        const next = [...reidentifications];
                        const recordsToUpdate = next.filter(x => x.pseudo === pseudoNumber);
                        recordsToUpdate.forEach(item => {
                            item.nhsnumber = result.identificationRequest?.identificationItems[0].identifier;
                            item.loading = false;
                        })
                        console.log(next);
                        return next;
                    });
                })
            }
        }
        execute()
    }, [identificationRequests, reidentifications, submit]);

    function reidentify(pseudoNumber: string) {
        if (isHx(pseudoNumber)) {
            pseudoNumber = convertHx(pseudoNumber)
        }

        const acc = accounts[0];

        const trackedAccessRequest: TrackedAccessRequest = {
            called: false,
            completed: false,
            accessRequest: {
                csvIdentificationRequest: undefined,
                impersonationContext: undefined,
                identificationRequest: {
                    id: crypto.randomUUID(),
                    identificationItems: [{
                        rowNumber: "1",
                        identifier: pseudoNumber,
                        hasAccess: undefined,
                        message: undefined,
                        isReidentified: undefined,
                    }],
                    DisplayName: acc.name || "",
                    GivenName: "TODO",
                    email: acc.username,
                    JobTitle: "TODO",
                    Organisation: "TODO",
                    Surname: "TODO",
                    reason: reason
                }
            }
        }

        setIdentificationRequests((identificationRequests) => [...identificationRequests, trackedAccessRequest])
        setLastPseudo(pseudoNumber);
    }

    function clearList() {
        setLastPseudo("");
        setIdentificationRequests([]);
        setReidentifications([]);
    }

    return { reidentify, reidentifications, lastPseudo, clearList }
}