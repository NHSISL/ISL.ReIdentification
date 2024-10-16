import { Guid } from "guid-typescript";
import { PdsData } from "../pds/pdsData";
export class OdsData {
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

    constructor(ods: any) {
        this.id = ods.id ? Guid.parse(ods.id) : Guid.parse(Guid.EMPTY);
        this.organisationCode_Root = ods.organisationCode_Root || "";
        this.organisationPrimaryRole_Root = ods.organisationPrimaryRole_Root || "";
        this.organisationCode_Parent = ods.organisationCode_Parent || "";
        this.organisationPrimaryRole_Parent = ods.organisationPrimaryRole_Parent || "";
        this.relationshipStartDate = ods.relationshipStartDate ? new Date(ods.relationshipStartDate) : undefined;
        this.relationshipEndDate = ods.relationshipEndDate ? new Date(ods.relationshipEndDate) : undefined;
        this.path = ods.path || "";
        this.depth = ods.depth;

        if (ods.pdsData !== undefined && ods.pdsData !== null) {
            this.pdsData = new PdsData(ods.pdsData);
        }
    }
}