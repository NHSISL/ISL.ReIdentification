import { useMutation } from '@tanstack/react-query';
import ReIdentificationBroker from "../../brokers/apiBroker.reIdentification";
import { AccessRequestView } from '../../models/views/components/accessRequest/accessRequestView';
import { AccessRequest } from '../../models/accessRequest/accessRequest';

export const reIdentificationService = {

    useRequestReIdentification: () => {
        const broker = new ReIdentificationBroker();

        return useMutation({
            mutationFn: (accessRequestView: AccessRequestView) => {
                return broker.PostReIdentificationAsync(accessRequestView);
            },
            onSuccess: (acessRequest: AccessRequest) => {
                return acessRequest;
            },
            onError: (error) => {
                console.error('Error during re-identification:', error);
            }
        });
    }
}