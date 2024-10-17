import { Guid } from "guid-typescript";
import { PdsData } from "../../../pds/pdsData";

export class OdsDataView {
    public id: Guid;
    public organisationCode_Root: string;
    public organisationPrimaryRole_Root: string;
    public organisationCode_Parent: string;
    public organisationPrimaryRole_Parent: string;
    public relationshipStartDate?: Date | undefined;
    public relationshipEndDate?: Date | undefined;
    public path?: string;
    public depth?: number;
    public pdsData?: PdsData

    constructor(
        id: Guid,
        organisationCode_Root: string,
        organisationPrimaryRole_Root: string,
        organisationCode_Parent: string,
        organisationPrimaryRole_Parent: string,
        relationshipStartDate?: Date,
        relationshipEndDate?: Date,
        path?: string,
        depth?: number,
        pdsData?: PdsData,
    ) {
        this.id = id;
        this.organisationCode_Root = organisationCode_Root;
        this.organisationPrimaryRole_Root = organisationPrimaryRole_Root;
        this.organisationCode_Parent = organisationCode_Parent;
        this.organisationPrimaryRole_Parent = organisationPrimaryRole_Parent;
        this.relationshipStartDate = relationshipStartDate;
        this.relationshipEndDate = relationshipEndDate;
        this.path = path;
        this.depth = depth;
        this.pdsData = pdsData;
    }
}