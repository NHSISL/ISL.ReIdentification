export class UserAccess {
    public id: string = "";
    public displayName: string = "";
    public entraGuid: string = "";
    public entraUpn: string = "";
    public userEmail: string = "";
    public orgCodes: string[] = [];
    public jobTitle: string = "";
    public activeFrom?: Date | undefined;
    public activeTo?: Date | undefined;
    public createdBy?: string;
    public createdDate?: Date | undefined;
    public updatedBy?: string;
    public updatedDate?: Date | undefined;
}