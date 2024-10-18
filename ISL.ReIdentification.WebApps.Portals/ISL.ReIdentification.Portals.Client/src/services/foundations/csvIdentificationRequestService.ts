import CsvIdentificationRequestBroker from "../../brokers/apiBroker.csvIdentificationRequest";
import { useMutation } from "@tanstack/react-query";
import { AccessRequestView } from "../../models/views/components/accessRequest/accessRequestView";
import { AccessRequest } from "../../models/accessRequest/accessRequest";


export const csvIdentificationRequestService = {
    useRequestCsvReIdentification: () => {
        const broker = new CsvIdentificationRequestBroker();

        return useMutation({
            mutationFn: (accessRequestView: AccessRequestView) => {
                return broker.PostCsvReIdentificationAsync(accessRequestView);
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