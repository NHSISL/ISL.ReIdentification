import { Guid } from 'guid-typescript';

export class ReidentificationRequest {
    public id: Guid;
    public requesterEmail: string;
    public recipientEmail: string;
    public isDelegatedAccess: boolean;
    public isApproved: boolean;
    public data: Uint8Array;
    public identifierColumn: string;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(reidentificationRequest: any) {
        this.id = reidentificationRequest.id ? Guid.parse(reidentificationRequest.id) : Guid.parse(Guid.EMPTY);
        this.requesterEmail = reidentificationRequest.requesterEmail || "";
        this.recipientEmail = reidentificationRequest.recipientEmail || "";
        this.isDelegatedAccess = reidentificationRequest.isDelegatedAccess || false;
        this.isApproved = reidentificationRequest.isApproved || false;
        this.data = reidentificationRequest.data || 0;
        this.identifierColumn = reidentificationRequest.identifierColumn || ""; 
        this.createdDate = reidentificationRequest.createdDate ? new Date(reidentificationRequest.createdDate) : undefined;
        this.createdBy = reidentificationRequest.createdBy || "";
        this.updatedDate = reidentificationRequest.updatedDate ? new Date(reidentificationRequest.updatedDate) : undefined;
        this.updatedBy = reidentificationRequest.updatedBy || "";
    }
}