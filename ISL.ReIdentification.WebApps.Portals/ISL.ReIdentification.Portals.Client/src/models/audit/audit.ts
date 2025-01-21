export class Audit {
    public id: string = "";
    public correlationId: string = "";
    public auditType: string = "";
    public auditDetail: string = "";
    public logLevel: string = "";
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}