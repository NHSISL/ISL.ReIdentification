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

    constructor(user?: UserAccess) {
        this.id = user && user.id ? user.id : crypto.randomUUID();
        this.entraGuid = user && user.entraGuid || "";
        this.entraUpn = user && user.entraUpn || "";
        this.displayName = user && user.displayName || "";
        this.userEmail = user && user.userEmail || "";
        this.orgCodes = user && user.orgCodes || [];
        this.jobTitle = user && user.jobTitle || "";
        this.activeFrom = user && user.activeFrom;
        this.activeTo = user && user.activeTo;
        this.createdDate = user && user.createdDate ? new Date(user.createdDate) : undefined;
        this.createdBy = user && user.createdBy || "";
        this.updatedDate = user && user.updatedDate ? new Date(user.updatedDate) : undefined;
        this.updatedBy = user && user.updatedBy || "";
    }
}