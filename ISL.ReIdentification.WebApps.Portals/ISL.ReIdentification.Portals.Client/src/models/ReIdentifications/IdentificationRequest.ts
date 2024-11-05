import { IdentificationItem } from "./IdentificationItem";

export class IdentificationRequest {
    public id: string = crypto.randomUUID();
    public identificationItems: IdentificationItem[] = [];
    public DisplayName: string = "";
    public Surname?: string = "";
    public GivenName?: string = "";
    public email: string = "";
    public JobTitle?: string = "";
    public Organisation?: string = "";
    public reason: string = "";
}
