export class UserAccessView {
    public id: string;
    public userEmail: string;
    public displayName: string;
    public entraGuid: string;
    public entraUpn: string;
    public jobTitle: string;
    public orgCodes: string[];
    public activeFrom?: Date;
    public activeTo?: Date;
    public createdBy?: string;
    public createdDate?: Date;
    public updatedBy?: string;
    public updatedDate?: Date;

    constructor(
        id: string,
        userEmail: string,
        displayName: string,
        entraGuid: string,
        entraUpn: string,
        jobTitle: string,
        orgCodes: string[],
        activeFrom?: Date,
        activeTo?: Date,
        createdBy?: string,
        createdDate?: Date,
        updatedBy?: string,
        updatedDate?: Date,
    ) {
        this.id = id;
        this.userEmail = userEmail || "";
        this.orgCodes = orgCodes || [];
        this.displayName = displayName;
        this.entraGuid = entraGuid;
        this.entraUpn = entraUpn;
        this.jobTitle = jobTitle;
        this.activeFrom = activeFrom;
        this.activeTo = activeTo;
        this.createdBy = createdBy !== undefined ? createdBy : '';
        this.createdDate = createdDate ;
        this.updatedBy = updatedBy !== undefined ? updatedBy : '';
        this.updatedDate = updatedDate;
    }
}