
export class entraUser {
    public id: string;
    public displayName: string;
    public jobTitle: string;
    public mail: string;
    public userPrincipalName: string;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;

    constructor(entraUser: entraUser) {
        this.id = entraUser.id;
        this.displayName= entraUser.displayName;
        this.jobTitle = entraUser.jobTitle;
        this.mail = entraUser.mail;
        this.userPrincipalName = entraUser.userPrincipalName;
        this.createdDate = entraUser.createdDate ? new Date(entraUser.createdDate) : undefined;
        this.createdBy = entraUser.createdBy || "";
        this.updatedDate = entraUser.updatedDate ? new Date(entraUser.updatedDate) : undefined;
        this.updatedBy = entraUser.updatedBy || "";
    }
}