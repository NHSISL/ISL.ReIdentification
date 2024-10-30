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
    },

    useRequestReIdentificationCsv: () => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        return {
            submit: (csvIdentificationRequest: AccessRequest) => {
                setIsLoading(true);
                return broker.PostReIdentificationCsvAsync(csvIdentificationRequest).finally(() => {
                    setIsLoading(false);
                })
            },
            loading
        };
    },

    useGetCsvIdentificationRequestById: (id: string) => {
        const broker = new ReIdentificationBroker();
        const [loading, setIsLoading] = useState(false);
        const [data, setData] = useState<AccessRequest | null>(null);
        const [error, setError] = useState<Error | null>(null);

        return {
            fetch: () => {
                setIsLoading(true);
                return broker.GetCsvIdentificationRequestByIdAsync(id)
                    .then(result => {
                        setData(result);
                        setError(null);
                    })
                    .catch(err => {
                        setError(err);
                        setData(null);
                    })
                    .finally(() => {
                        setIsLoading(false);
                    });
            },
            loading,
            data,
            error
        };
    }
}