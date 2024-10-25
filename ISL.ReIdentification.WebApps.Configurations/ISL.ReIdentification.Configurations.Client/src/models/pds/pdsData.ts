
export class PdsData {
    public rowId: string;
    public pseudoNhsNumber: string;
    public OrgCode: string;
    public OrganisationName: string;
    public RelationshipWithOrganisationEffectiveFromDate?: Date | undefined;
    public RelationshipWithOrganisationEffectiveToDate?: Date | undefined;

    constructor(pds: PdsData) {
            this.rowId = pds.rowId;
            this.pseudoNhsNumber = pds.pseudoNhsNumber;
            this.OrgCode = pds.OrgCode;
            this.OrganisationName = pds.OrganisationName
            this.RelationshipWithOrganisationEffectiveFromDate = pds.RelationshipWithOrganisationEffectiveFromDate
            this.RelationshipWithOrganisationEffectiveToDate = pds.RelationshipWithOrganisationEffectiveToDate;
    }
}