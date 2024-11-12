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
    public organisation?: string[];
    public hasHeaderRecord?: boolean;
    public identifierColumnIndex?: number;
    public createdDate?: Date;

    constructor(
        id?: string,
        requesterEntraUserId?: string,
        requesterFirstName?: string,
        requesterLastName?: string,
        requesterDisplayName?: string,
        requesterEmail?: string,
        recipientEntraUserId?: string,
        recipientFirstName?: string,
        recipientLastName?: string,
        recipientDisplayName?: string,
        recipientEmail?: string,
        reason?: string,
        organisation?: string[],
        hasHeaderRecord?: boolean,
        identifierColumnIndex?: number,
        createdDate?: Date,
    ) {
        this.id = id;
        this.requesterEntraUserId = requesterEntraUserId || ""
        this.requesterFirstName = requesterFirstName || "";
        this.requesterLastName = requesterLastName || "";
        this.requesterDisplayName = requesterDisplayName || "";
        this.requesterEmail = requesterEmail || "";
        this.recipientEntraUserId = recipientEntraUserId || "";
        this.recipientFirstName = recipientFirstName || "";
        this.recipientLastName = recipientLastName || "";
        this.recipientDisplayName = recipientDisplayName || "";
        this.recipientEmail = recipientEmail || "";
        this.reason = reason || "";
        this.organisation = organisation || [""];
        this.hasHeaderRecord = hasHeaderRecord || false;
        this.identifierColumnIndex = identifierColumnIndex;
        this.createdDate = createdDate;
    }
}