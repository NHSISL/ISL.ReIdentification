export class UserAccess {
    public id: string;
    public firstName: string;
    public lastName: string;
    public userEmail: string;
    public orgCode: string;
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(user: any) {
        this.id = user.id ? user.id : "";
        this.firstName = user.firstName || "";
        this.lastName = user.lastName || "";
        this.userEmail = user.userEmail || "";
        this.orgCode = user.orgCode || "";
        this.activeFrom = user.activeFrom;
        this.activeTo = user.activeTo;
        this.createdDate = user.createdDate ? new Date(user.createdDate) : undefined;
        this.createdBy = user.createdBy || "";
        this.updatedDate = user.updatedDate ? new Date(user.updatedDate) : undefined;
        this.updatedBy = user.updatedBy || "";
    }
}