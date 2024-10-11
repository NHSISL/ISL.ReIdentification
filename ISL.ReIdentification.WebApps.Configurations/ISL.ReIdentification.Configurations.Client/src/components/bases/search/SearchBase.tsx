import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React, { FunctionComponent, FormEventHandler } from "react";

interface SearchBaseProps {
    id: string;
    label?: string;
    onChange: FormEventHandler<HTMLInputElement>;
    placeholder?: string;
    value?: string | number;
    description?: string;
}

const SearchBase: FunctionComponent<SearchBaseProps> = (props) => {
    return (
        <>
            <input
                id="input-example"
                placeholder={props.placeholder}
                value={props.value}
                type="search"
                className="form-control"
                onChange={props.onChange}>
            </input>

            {props.description && (<><br /><small>{props.description}</small></>)}
        </>
    );
}

export default SearchBase;