export class ImpersonationContextView {
    public id: string;
    public requesterEntraUserId: string;
    public requesterFirstName: string;
    public requesterLastName: string;
    public requesterDisplayName: string;
    public requesterEmail: string;
    public requesterJobTitle: string;
    public responsiblePersonEntraUserId: string;
    public responsiblePersonFirstName: string;
    public responsiblePersonLastName: string;
    public responsiblePersonDisplayName: string;
    public responsiblePersonEmail: string;
    public responsiblePersonJobTitle: string;
    public reason: string;
    public purpose: string;
    public organisation: string;
    public projectName: string;
    public inboxToken: string;
    public outboxToken: string;
    public errorToken: string;
    public isApproved: boolean;
    public identifierColumn: string;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(
        id: string,
        requesterEntraUserId: string,
        requesterFirstName: string,
        requesterLastName: string,
        requesterDisplayName: string,
        requesterEmail: string,
        requesterJobTitle: string,
        responsiblePersonEntraUserId: string,
        responsiblePersonFirstName: string,
        responsiblePersonLastName: string,
        responsiblePersonDisplayName: string,
        responsiblePersonEmail: string,
        responsiblePersonJobTitle: string,
        reason: string,
        purpose: string,
        organisation: string,
        projectName: string,
        inboxToken: string,
        outboxToken: string,
        errorToken: string,
        isApproved: boolean,
        identifierColumn: string,
        createdBy?: string,
        createdDate?: Date | undefined,
        updatedBy?: string,
        updatedDate?: Date | undefined,
    ) {
        this.id = id;
        this.requesterEntraUserId = requesterEntraUserId || "";
        this.requesterFirstName = requesterFirstName || "";
        this.requesterLastName = requesterLastName || "";
        this.requesterDisplayName = requesterDisplayName || "";
        this.requesterEmail = requesterEmail || "";
        this.requesterJobTitle = requesterJobTitle || "";
        this.responsiblePersonEntraUserId = responsiblePersonEntraUserId || "";
        this.responsiblePersonFirstName = responsiblePersonFirstName || "";
        this.responsiblePersonLastName = responsiblePersonLastName || "";
        this.responsiblePersonDisplayName = responsiblePersonDisplayName || "";
        this.responsiblePersonEmail = responsiblePersonEmail || "";
        this.responsiblePersonJobTitle = responsiblePersonJobTitle || "";
        this.reason = reason || "";
        this.purpose = purpose || "";
        this.organisation = organisation || "";
        this.projectName = projectName || "";
        this.inboxToken = inboxToken || "";
        this.outboxToken = outboxToken || "";
        this.errorToken = errorToken || "";
        this.isApproved = isApproved || false;
        this.identifierColumn = identifierColumn || "";
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}