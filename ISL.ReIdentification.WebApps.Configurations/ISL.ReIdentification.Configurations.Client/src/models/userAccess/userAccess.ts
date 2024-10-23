export class UserAccess {
    public id: string;
    public displayName: string;
    public entraGuid: string;
    public entraUpn: string;
    public userEmail: string;
    public orgCodes: string[];
    public jobTitle: string;
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(user: any) {
        this.id = user.id ? user.id : crypto.randomUUID();
        this.entraGuid = user.entraGuid || "";
        this.entraUpn = user.entraUpn || "";
        this.displayName = user.displayName || "";
        this.userEmail = user.userEmail || "";
        this.orgCodes = user.orgCodes || "";
        this.jobTitle = user.jobTitle || "";
        this.activeFrom = user.activeFrom;
        this.activeTo = user.activeTo;
        this.createdDate = user.createdDate ? new Date(user.createdDate) : undefined;
        this.createdBy = user.createdBy || "";
        this.updatedDate = user.updatedDate ? new Date(user.updatedDate) : undefined;
        this.updatedBy = user.updatedBy || "";
    }
}