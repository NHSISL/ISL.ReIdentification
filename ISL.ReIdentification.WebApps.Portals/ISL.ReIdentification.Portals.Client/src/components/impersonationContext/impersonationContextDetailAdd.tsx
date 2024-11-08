import React, { FunctionComponent } from "react";

type ImpersonationContextDetailAddProps = {
    children?: React.ReactNode;
};

const ImpersonationContextDetailAdd: FunctionComponent<ImpersonationContextDetailAddProps> = (props) => {
    const {
        children
    } = props;


    return (
        <div>
            POW - Add  Me
        </div>
    );
};

export default ImpersonationContextDetailAdd;