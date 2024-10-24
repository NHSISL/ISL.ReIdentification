export class OdsDataView {
    public id: string;
    public organisationCode: string;
    public organisationName: string;
    public relationshipStartDate?: Date | undefined;
    public relationshipEndDate?: Date | undefined;
    public hasChildren: boolean;

    constructor(
        id: string,
        organisationCode: string,
        organisationName: string,
        hasChildren: boolean,
        relationshipStartDate?: Date,
        relationshipEndDate?: Date
    ) {
        this.id = id;
        this.organisationCode = organisationCode;
        this.organisationName = organisationName;
        this.hasChildren = hasChildren;
        this.relationshipStartDate = relationshipStartDate;
        this.relationshipEndDate = relationshipEndDate;
    }
}