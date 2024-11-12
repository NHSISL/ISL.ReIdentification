export class CsvIdentificationRequestView {
    public id?: string;
    public requesterEntraUserId?: string;
    public requesterFirstName?: string;
    public requesterLastName?: string;
    public requesterDisplayName?: string;
    public requesterEmail?: string;
    public recipientEntraUserId?: string;
    public recipientFirstName?: string;
    public recipientLastName?: string;
    public recipientDisplayName?: string;
    public recipientEmail?: string;
    public reason?: string;
    public organisation?: string;
    public hasHeaderRecord?: boolean;
    public identifierColumnIndex?: number;
    public createdDate?: Date;
}