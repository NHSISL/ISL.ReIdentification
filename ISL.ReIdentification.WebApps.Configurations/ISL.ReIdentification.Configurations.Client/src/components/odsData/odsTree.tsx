import React, { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { Button, Form, FormCheck } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { width } from "@fortawesome/free-solid-svg-icons/faUserFriends";

type OdsTreeProps = {
    parentId: string;
};

type OdsTreeElementProps = {
    node: OdsData,
    addSelectedRecord: (orgCode: string) => void,
    removeSelectedRecord: (orgCode: string) => void,
    parentSelected: boolean
}

const OdsTreeElement: FunctionComponent<OdsTreeElementProps> = ({node, addSelectedRecord, removeSelectedRecord, parentSelected}) => {
    const [isExpanded, setIsExpanded] = useState(false);
    const [expandedId,setExpandedId] = useState("");
    const { data, isLoading } = odsDataService.useGetOdsChildren(expandedId);
    const [selected, setIsSelected] = useState(false);

    const toggle = (id: string) =>  {
        if (isExpanded) {
            setIsExpanded(false)
            setExpandedId("")
        } else {
            setIsExpanded(true);
            setExpandedId(id);
        }
    }


    const processCheck = (event: React.ChangeEvent<HTMLInputElement> ) => {
        console.log(event);
        if(event.target.checked) {
            addSelectedRecord(node.organisationCode)
            setIsSelected(true);
        } else {
            removeSelectedRecord(node.organisationCode);
            setIsSelected(false);
        }
    }

    return (<div style={{paddingLeft:'30px'}}>
        <span style={{width: "20px", display:"inline-block"}}>
            {node.hasChildren ? 
                <span onClick={() => {toggle(node.id)}} >
                    {isExpanded ? <FontAwesomeIcon icon={faMinus}/> : <FontAwesomeIcon icon={faPlus}/>}
                </span>
                :
                ""
            }
        </span>

        <span>{node.organisationCode}</span>
        &nbsp;
        <span><Form.Check inline onChange={processCheck} disabled={parentSelected} checked={selected || parentSelected}/></span>
        {isLoading && <FontAwesomeIcon icon={faSpinner} pulse/>}
        {data && data.map((element: OdsData) => <>
            <OdsTreeElement node={element} key={element.id} addSelectedRecord={addSelectedRecord} removeSelectedRecord={removeSelectedRecord} parentSelected={parentSelected || selected}></OdsTreeElement>
        </>)}
    </div>)
}


const OdsTree: FunctionComponent<OdsTreeProps> = ({ parentId }) => {
    const { data } = odsDataService.useGetOdsChildren(parentId);
    const [selectedOdsRecords, setSetSelectedOdsRecords] = useState<Array<string>>([]);
    const [isSelected, setIsSelected] = useState(false);

    const addSelectedRecord = (odsRecord: string) =>
    {
        setSetSelectedOdsRecords([...selectedOdsRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: string) => {
        setSetSelectedOdsRecords([...selectedOdsRecords.filter(o => o != odsRecord)])
    }

    return (
        <>
            {data && data.map((element: OdsData) => <> 
                <OdsTreeElement node={element} addSelectedRecord={addSelectedRecord} removeSelectedRecord={removeSelectedRecord} parentSelected={isSelected}></OdsTreeElement>
            </>
            )}

            selected records = {JSON.stringify(selectedOdsRecords)}
        </>
    );
};

export default OdsTree;