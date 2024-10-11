import { Guid } from 'guid-typescript';

export class ImpersonationContext {
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

    constructor(impersonationContext: any) {
        this.id = impersonationContext.id ? Guid.parse(impersonationContext.id) : Guid.parse(Guid.EMPTY);
        this.requesterEmail = impersonationContext.requesterEmail || "";
        this.recipientEmail = impersonationContext.recipientEmail || "";
        this.isDelegatedAccess = impersonationContext.isDelegatedAccess || false;
        this.isApproved = impersonationContext.isApproved || false;
        this.data = impersonationContext.data || 0;
        this.identifierColumn = impersonationContext.identifierColumn || ""; 
        this.createdDate = impersonationContext.createdDate ? new Date(impersonationContext.createdDate) : undefined;
        this.createdBy = impersonationContext.createdBy || "";
        this.updatedDate = impersonationContext.updatedDate ? new Date(impersonationContext.updatedDate) : undefined;
        this.updatedBy = impersonationContext.updatedBy || "";
    }
}