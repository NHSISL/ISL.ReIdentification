import { ErrorBase } from "../../../types/ErrorBase";
export interface ILookupErrors extends ErrorBase {
    hasErrors: boolean;
    name: string;
    value: string;
}

export const lookupErrors: ILookupErrors = {
    hasErrors: false,
    name: "",
    value: ""
};
