import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../models/accessRequest/accessRequest";
import {  useEffect, useState } from "react";
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
    const { submit, loading, data } = reIdentificationService.useRequestReIdentification();

    useEffect(() => {
        console.log(data);
    },[data])

    const processRequest = async (request: TrackedAccessRequest) => {

        if (request.accessRequest.identificationRequest) {
            setIsLoading(true);
            await submit(request.accessRequest);
            setIsLoading(false);
        }
    }

    const reidentify = async (pseudoNumbers: string[]) => {
        const acc = accounts[0];

        const identificationItems = pseudoNumbers.map((ps, i): IdentificationItem => {
            return {
                rowNumber: crypto.randomUUID(),
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

    return { reidentify, reidentifications: data, lastPseudo, clearList, isLoading }
}


