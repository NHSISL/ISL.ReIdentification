import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../models/accessRequest/accessRequest";
import {  useState } from "react";
import { ReIdRecord } from "../types/ReIdRecord";
import { reIdentificationService } from "../services/foundations/reIdentificationService";
import { IdentificationItem } from "../models/ReIdentifications/IdentificationItem";

type TrackedAccessRequest = {
    called: boolean,
    completed: boolean,
    accessRequest: AccessRequest,
}

interface Dictionary<T> {
    [index: string]: T
}

class ReidentificationCache {
    items: Dictionary<ReIdRecord> = {}
}

export function useReidentification(reason: string) {
    const [reidentifications, setReidentifications] = useState<ReIdRecord[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [lastPseudo, setLastPseudo] = useState<ReIdRecord>();
    const { accounts } = useMsal();
    const { submit } = reIdentificationService.useRequestReIdentification();

    function isHx(hxNumber: string) {
        return hxNumber.indexOf("-") !== -1
    }

    function convertHx(hxNumber: string) {
        const cleanedID = hxNumber.replace(/-/g, '');

        const originalHex = cleanedID.split('').reverse().join('');

        return parseInt(originalHex, 16).toString();
    }

    const processRequest = async (request: TrackedAccessRequest) => {

        if (request.accessRequest.identificationRequest) {

            setReidentifications((reidentifications) => {
                const nextX = [...reidentifications];
                request.accessRequest.identificationRequest?.identificationItems.forEach(x => {
                    const origId = savedIdentificationItems.find(ii => ii.rowNumber === x.rowNumber)?.identifier || "";
                    nextX.unshift({
                        identifier: x.identifier,
                        pseudo: origId || "",
                        hasAccess: x.hasAccess,
                        nhsnumber: "",
                        loading: true,
                        rowNumber: x.rowNumber,
                        isHx: isHx(origId)
                    })
                }
                )
                return nextX;
            });

            const savedIdentificationItems = [...request.accessRequest.identificationRequest.identificationItems]
            request.accessRequest.identificationRequest.identificationItems = request.accessRequest.identificationRequest.identificationItems.map(ii => {
                return {
                    ...ii,
                    identifier: `0000000000000${isHx(ii.identifier) ? convertHx(ii.identifier) : ii.identifier}`.slice(-10),
                }
            });

            setIsLoading(true);
            const result = await submit(request.accessRequest);

            setReidentifications((reidentifications) => {
                const nextX = [...reidentifications];
                result.identificationRequest?.identificationItems.forEach(x => {
                    const origId = savedIdentificationItems.find(ii => ii.rowNumber === x.rowNumber)?.identifier;

                    if (origId) {
                        const index = nextX.findIndex(filter => filter.pseudo === origId);
                        nextX[index] = { ...nextX[index], nhsnumber: x.identifier, loading: false, hasAccess: x.hasAccess }
                    }
                })
                return nextX;
            });

            setIsLoading(false);
        }
    }

    const reidentify = async (pseudoNumbers: string[]) => {
        const acc = accounts[0];

        const identificationItems = pseudoNumbers.map((ps, i): IdentificationItem => {
            return {
                rowNumber: "" + i,
                identifier: ps,
                hasAccess: false
            }
        });

        const trackedAccessRequest: TrackedAccessRequest = {
            called: false,
            completed: false,
            accessRequest: {
                csvIdentificationRequest: undefined,
                impersonationContext: undefined,
                identificationRequest: {
                    id: crypto.randomUUID(),
                    identificationItems: identificationItems,
                    displayName: acc.name || "",
                    email: acc.username,
                    reason: reason,
                    organisation: "TODO"
                }
            }
        }

        await processRequest(trackedAccessRequest)
    }

    function clearList() {
        setLastPseudo(undefined);
        setReidentifications([]);
    }

    return { reidentify, reidentifications, lastPseudo, clearList, isLoading }
}


