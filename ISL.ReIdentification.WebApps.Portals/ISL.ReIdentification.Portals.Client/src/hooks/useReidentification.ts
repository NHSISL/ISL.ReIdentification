import { useMsal } from "@azure/msal-react";
import { AccessRequest } from "../models/accessRequest/accessRequest";
import { useEffect, useState } from "react";
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
    const [identificationRequests, setIdentificationRequests] = useState<TrackedAccessRequest[]>([]);
    const [reidentificationCache, setReidentificationCache] = useState<ReidentificationCache>(new ReidentificationCache());
    const [lastPseudo, setLastPseudo] = useState("");
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

    useEffect(() => {
        async function execute() {
            // process any new re-id requests
            const reidentificationsToCall = identificationRequests.filter(x => !x.called)
            const next = [...identificationRequests];

            if (reidentificationsToCall) {
                //if there are any loop through them all calling them.
                reidentificationsToCall.forEach(async x => {
                    x.called = true;
                    setIdentificationRequests(next);

                    // push all the items on to the re-identification list correct mapped to a ReId Record
                    setReidentifications((reidentifications) => {
                        
                        let nextReid: ReIdRecord[] = [...reidentifications];
                        const items = x.accessRequest.identificationRequest?.identificationItems.map(x => {
                            const item = {
                                identifier: isHx(x.identifier) ? convertHx(x.identifier) : x.identifier,
                                loading: true,
                                hasAccess: false,
                                pseudo: x.identifier,
                                rowNumber: x.rowNumber,
                                isHx: isHx(x.identifier),
                                identifer: crypto.randomUUID()
                            } as ReIdRecord

                            // add the items to the cache (will return values without calling the api)
                            // pushed early to cache as will be a referenced value so even if call is in progress we will hydrate after loading
                            setReidentificationCache((cache) => {
                                cache.items[x.identifier] = item;
                                return cache;
                            })

                            return item;
                        }
                        );

                        if (items) {
                            //concate new items all to the existing reidentifications.
                            nextReid = items.concat(nextReid)
                        }
                        return nextReid;
                    });


                    // process any cache hits and update the reidentifications list.
                    const cacheHits = x.accessRequest.identificationRequest?.identificationItems.filter(x => reidentificationCache.items[x.identifier]);

                    if (cacheHits?.length) {
                        setReidentifications((reidentifications) => {
                            const nextX = [...reidentifications];
                            cacheHits.forEach(res => {
                                const recordsToUpdate = nextX.filter(x => x.rowNumber === res.rowNumber)
                                recordsToUpdate.forEach(item => {
                                    const values = reidentificationCache.items[item.pseudo]
                                    item.nhsnumber = values.nhsnumber;
                                    item.loading = false;
                                    item.hasAccess = values.hasAccess;
                                })
                            })
                            return nextX;
                        });
                    }


                    // process cache misses.
                    const cacheMisses = x.accessRequest.identificationRequest?.identificationItems.filter(x => !reidentificationCache.items[x.identifier]);

                    if (cacheMisses?.length && x.accessRequest.identificationRequest) {

                        // replace the full set of identification items with only the missed records.
                        x.accessRequest.identificationRequest.identificationItems = cacheMisses;
                        
                        //call the api
                        const result = await submit(x.accessRequest);
                        x.completed = true;

                        // process the responses into the reidentification queue.
                        setReidentifications((reidentifications) => {
                            const nextX = [...reidentifications];
                            //const request = next.filter(r => r.)
                            result.identificationRequest?.identificationItems.forEach(res => {
                                const recordsToUpdate = nextX.filter(x => x.rowNumber === res.rowNumber)
                                recordsToUpdate.forEach(item => {
                                    item.nhsnumber = res.identifier;
                                    item.loading = false;
                                    item.hasAccess = res.hasAccess;
                                })
                            })
                            return nextX;
                        });
                    }
                })
            }
        }
        execute()
    }, [identificationRequests, reidentificationCache.items, reidentifications, submit]);

    function reidentify(pseudoNumbers: string[]) {
        const acc = accounts[0];

        const identificationItems = pseudoNumbers.map((ps, i) => {
            return {
                rowNumber: "" + i,
                identifier: ps
            } as IdentificationItem
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
                    DisplayName: acc.name || "",
                    email: acc.username,
                    reason: reason
                }
            }
        }

        setIdentificationRequests((identificationRequests) => [...identificationRequests, trackedAccessRequest])
        if (pseudoNumbers.length === 1) {
            setLastPseudo(pseudoNumbers[0]);
        } else {
            setLastPseudo("");
        }

    }

    function clearList() {
        setLastPseudo("");
        setIdentificationRequests([]);
        setReidentifications([]);
    }

    return { reidentify, reidentifications, lastPseudo, clearList }
}