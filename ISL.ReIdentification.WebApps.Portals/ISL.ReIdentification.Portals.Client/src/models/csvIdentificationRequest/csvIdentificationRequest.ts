export class CsvIdentificationRequest {
    public id?: string = "";
    public requesterEntraUserId?: string = "";
    public requesterFirstName?: string = "";
    public requesterLastName?: string = "";
    public requesterDisplayName?: string = "";
    public requesterEmail?: string = "";
    public requesterJobTitle?: string = "";
    public recipientEntraUserId?: string = "";
    public recipientFirstName?: string = "";
    public recipientLastName?: string = "";
    public recipientDisplayName?: string = "";
    public recipientEmail?: string = "";
    public recipientJobTitle?: string = "";
    public reason?: string = "";
    public organisation?: string = "";
    public data?: Uint8Array = new Uint8Array();
    public sha256Hash?: string = "";
    public identifierColumn?: string = "";
    public createdBy?: string = "";
    public createdDate?: Date | undefined;
    public updatedBy?: string = "";
    public updatedDate?: Date | undefined;
}