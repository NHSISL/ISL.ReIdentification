export class OdsData {
    public id: string;
    public organisationCode: string;
    public organisationName: string;
    public relationshipStartDate?: Date | undefined;
    public relationshipEndDate?: Date | undefined;
    public hasChildren: boolean;

    constructor(ods: OdsData) {
        this.id = ods.id ?? crypto.randomUUID();
        this.organisationCode = ods.organisationCode || "";
        this.organisationName = ods.organisationName || "";
        this.relationshipStartDate = ods.relationshipStartDate ? new Date(ods.relationshipStartDate) : undefined;
        this.relationshipEndDate = ods.relationshipEndDate ? new Date(ods.relationshipEndDate) : undefined;
        this.hasChildren = ods.hasChildren || false;
    }
}