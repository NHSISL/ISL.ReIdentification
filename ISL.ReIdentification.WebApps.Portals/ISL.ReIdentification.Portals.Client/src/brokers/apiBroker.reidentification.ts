import { AccessRequest } from "../models/accessRequest/accessRequest";
import ApiBroker from "./apiBroker";

class SimpleReIdentificationBroker {
    relativeReIdentificationUrl = '/api/reIdentification/';
    relativeReIdentificationCsvUrl = '/api/reIdentification/submitcsv';
    relativeCsvReIdentificationUrl = '/api/reIdentification/csvreidentification';
    relativeImpersonationReIdentificationUrl = '/api/reIdentification/impersonation';
    relativeImpersonationReIdentificationGenerateUrl = '/api/reIdentification/generatetokens';

    private apiBroker: ApiBroker = new ApiBroker();

    async PostReIdentificationAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async PostReIdentificationCsvAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeReIdentificationCsvUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async PostReIdentificationImpersonationAsync(accessRequestView: AccessRequest) {
        return await this.apiBroker.PostAsync(this.relativeImpersonationReIdentificationUrl, accessRequestView)
            .then(result => result.data as AccessRequest);
    }

    async PostImpersonationContextGenerateTokensAsync(impersonationContextId: string) {
        alert(impersonationContextId);
        return await this.apiBroker.PostAsync(
            this.relativeImpersonationReIdentificationGenerateUrl,
            {
                body: JSON.stringify({ impersonationContextId }),
                headers: { "Content-Type": "application/json" },
            }
        )
            .then(result => result.data as AccessRequest);
    }



    async GetCsvIdentificationRequestByIdAsync(id: string, reason: string): Promise<{ data: AccessRequest, filename: string }> {
        const url = `${this.relativeCsvReIdentificationUrl}/${id}/${reason}`;

        try {
            const result = await this.apiBroker.GetAsync(url);
            const contentDisposition = result.headers['content-disposition'];
            let filename = 'reidentification.csv';

            if (contentDisposition) {
                const matches = contentDisposition.match(/filename\*?=['"]?UTF-8''([^;]+)['"]?/);
                if (matches && matches[1]) {
                    filename = decodeURIComponent(matches[1]);
                }
            }

            return { data: result.data as AccessRequest, filename };
        } catch (error) {
            console.error("Error fetching CSV Identification Request by ID:", error);
            throw error;
        }
    }
}

export default SimpleReIdentificationBroker;