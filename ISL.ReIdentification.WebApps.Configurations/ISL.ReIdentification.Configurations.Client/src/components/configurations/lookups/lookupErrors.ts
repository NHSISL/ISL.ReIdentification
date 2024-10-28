import { ErrorBase } from "../../../types/ErrorBase";
export interface ILookupErrors extends ErrorBase {
    hasErrors: boolean;
    id: string;
    name: string;
    value:string;
    createdDate: string;
    createdBy: string;
    updatedBy: string;
    updatedDate: string;
}


export const LookupErrors: ILookupErrors = {
    hasErrors: false,
    id:"",
    name: "",
    value: "",
    createdDate: "",
    createdBy:"",
    updatedBy:"",
    updatedDate:""
};
