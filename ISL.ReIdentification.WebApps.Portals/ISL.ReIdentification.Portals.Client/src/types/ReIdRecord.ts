export class  ReIdRecord {
    pseudo: string = "";
    nhsnumber?: string;
    loading: boolean = true;
    hasAccess: boolean = false;   
    rowNumber: string = ""; 
    isHx: boolean = false;
    identifer: string = crypto.randomUUID();
}
