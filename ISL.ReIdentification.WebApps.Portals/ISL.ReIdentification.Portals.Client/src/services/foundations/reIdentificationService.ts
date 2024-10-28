import { useState } from 'react';
import ReIdentificationBroker from '../../brokers/apiBroker.reidentification';
import { AccessRequest } from '../../models/accessRequest/accessRequest';

export const reIdentificationService = {
    

    useRequestReIdentification: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        return {
            submit: (identificationRequest: AccessRequest) => {
                setIsLoading(true);
                return broker.PostReIdentificationAsync(identificationRequest).finally(() =>
                {
                    setIsLoading(false);
                })
            },
            loading
        };
    }
}