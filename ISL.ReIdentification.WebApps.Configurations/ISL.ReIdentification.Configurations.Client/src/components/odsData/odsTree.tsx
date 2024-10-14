import React, { FunctionComponent } from "react";

type OdsTreeProps = {};

const OdsTree: FunctionComponent<OdsTreeProps> = (props) => {
    const topLevel = [
        'Foo',
        'Bar'
    ]


    return (
        <>
            WOW
            {topLevel.map(element => {
                return <div>{element}</div>
            })}
        </>
    );
};

export default OdsTree;