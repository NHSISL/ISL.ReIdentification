
export class PdsDataView {
    public rowId: string;
    public pseudoNhsNumber: string;
    public OrgCode: string;
    public OrganisationName: string;
    public RelationshipWithOrganisationEffectiveFromDate?: Date | undefined;
    public RelationshipWithOrganisationEffectiveToDate?: Date | undefined;

    constructor(
        rowId: string,
        pseudoNhsNumber: string,
        OrgCode: string,
        OrganisationName: string,
        RelationshipWithOrganisationEffectiveFromDate?: Date,
        RelationshipWithOrganisationEffectiveToDate?: Date,
    ) {
        this.rowId = rowId;
        this.pseudoNhsNumber = pseudoNhsNumber;
        this.OrgCode = OrgCode;
        this.OrganisationName = OrganisationName
        this.RelationshipWithOrganisationEffectiveFromDate = RelationshipWithOrganisationEffectiveFromDate
        this.RelationshipWithOrganisationEffectiveToDate = RelationshipWithOrganisationEffectiveToDate;
    }
}