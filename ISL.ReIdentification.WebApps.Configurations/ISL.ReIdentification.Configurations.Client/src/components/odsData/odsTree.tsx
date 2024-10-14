import React, { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { Button } from "react-bootstrap";

type OdsTreeProps = {
    parentId: string;
};

const OdsTree: FunctionComponent<OdsTreeProps> = ({ parentId }) => {
    console.log(parentId);
    const { data } = odsDataService.useGetOdsChildren(parentId);
    const [isExpanded, setIsExpanded] = useState(false);
    const [expandedId, setExpandedId] = useState<string>("");

    const toggle = (id: string) =>  {
        console.log(id);
        if (isExpanded) {
            setIsExpanded(false)
            setExpandedId("")
        } else {
            setIsExpanded(true);
            setExpandedId(id);
        }
    }

    return (
        <>
            {data && data.map((element: OdsData) => {
                return <div>
                    <div>
                        {isExpanded ? "[-]" : "[+]"}
                        <span key={element.id} >x: {element.organisationCode}</span>
                        <Button onClick={() => {toggle(element.id)}}>exp</Button>
                    </div>
                </div>
            })}


        </>


    );
};

export default OdsTree;