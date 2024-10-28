export class ImpersonationContext {
    public id: string = "";
    public requesterFirstName: string = "";
    public requesterLastName: string = "";
    public requesterEmail: string = "";
    public recipientFirstName: string = "";
    public recipientLastName: string = "";
    public recipientEmail: string = "";
    public reason: string = "";
    public purpose: string = "";
    public organisation: string = "";
    public isApproved: boolean = false
    public identifierColumn: string = "";
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}