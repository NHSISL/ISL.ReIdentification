export class UserAccessView {
    public id: string;
    public firstName: string;
    public lastName: string;
    public userEmail: string;
    public orgCodes: string[];
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        firstName: string,
        lastName: string,
        userEmail: string,
        orgCodes: string[],
        activeFrom?: Date,
        activeTo?: Date,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date,
    ) {
        this.id = id;
        this.firstName = firstName || "";
        this.lastName = lastName || "";
        this.userEmail = userEmail || "";
        this.orgCodes = orgCodes || [];
        this.activeFrom = activeFrom;
        this.activeTo = activeTo;
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate ;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}