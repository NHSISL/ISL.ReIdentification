import { useEffect, useState } from 'react';
import ReIdentificationBroker from '../../brokers/apiBroker.reidentification';
import { AccessRequest } from '../../models/accessRequest/accessRequest';
import { IdentificationItem } from '../../models/ReIdentifications/IdentificationItem';
import { ReIdRecord } from '../../types/ReIdRecord';
import { faL } from '@fortawesome/free-solid-svg-icons';

export const reIdentificationService = {



    useRequestReIdentification: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        const [request, setRequest] = useState<AccessRequest>({});
        const [data, setData] = useState<ReIdRecord[]>([]);

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
                if (request && request.identificationRequest && request.identificationRequest?.identificationItems) {
                    setIsLoading(true);

                    console.log(request.identificationRequest.identificationItems.length);
                    const reIdRecords: ReIdRecord[] = request.identificationRequest?.identificationItems.map(ii => {
                        return {
                            identifier: isHx(ii.identifier) ? convertHx(ii.identifier) : ii.identifier,
                            pseudo: ii.identifier,
                            hasAccess: ii.hasAccess,
                            nhsnumber: "",
                            loading: true,
                            rowNumber: ii.rowNumber,
                            isHx: isHx(ii.identifier)
                        }
                    })

                    request.identificationRequest.identificationItems = 
                            request.identificationRequest.identificationItems.map(i => 
                                    {return { ...i, 
                                        identifier: `0000000000000${isHx(i.identifier) ? convertHx(i.identifier) : i.identifier}`.slice(-10) }});

                    request.identificationRequest.identificationItems = request.identificationRequest.identificationItems.filter(x => data.findIndex(y => y.identifier === x.identifier) === -1);
                    
                    if(request.identificationRequest.identificationItems.length === 0){
                        return;
                    }

                    
                    await broker.PostReIdentificationAsync(request)
                        .then((response) => {
                            setData((data) => {
                                const responseItems = response.identificationRequest?.identificationItems;
                                if(!responseItems) {
                                    return data;
                                }

                                const foo : ReIdRecord[] =  responseItems.map(x => {
                                    const r = reIdRecords.find(cachedRecord => cachedRecord.rowNumber === x.rowNumber);
                                    return { ...r, 
                                        hasAccess: x.hasAccess,
                                        nhsnumber: x.identifier,
                                        loading: false,
                                    }
                                })

                                console.log(reIdRecords);
                                console.log(foo);

                                return [...data,...foo,]

                            })
                        }).finally(() => {
                            setIsLoading(false);
                        })


                }
            }

            execute();

        }, [request]);


        return {
            submit: (identificationRequest: AccessRequest) => {
                setIsLoading(true);
                setRequest(identificationRequest);
            },
            loading,
            data
        };
    }
}