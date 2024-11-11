import { faMinus, faPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { OdsData } from "../../models/odsData/odsData";
import { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { Form } from "react-bootstrap";

type OdsTreeElementProps = {
    node: OdsData,
    addSelectedRecord: (orgCode: OdsData) => void,
    removeSelectedRecord: (orgCode: OdsData) => void,
    parentSelected: boolean,
    selectedRecords: OdsData[]
}


export const OdsTreeElement: FunctionComponent<OdsTreeElementProps> = ({ node, addSelectedRecord, removeSelectedRecord, parentSelected, selectedRecords }) => {
    const [isExpanded, setIsExpanded] = useState(false);
    const [expandedId, setExpandedId] = useState("");
    const { data, isLoading } = odsDataService.useGetOdsChildren(expandedId);
    const [selected, setIsSelected] = useState(false);
    const [selectedChildren, setSelectedChildren] = useState<OdsData[]>([]); 

    const toggle = (id: string) => {
        if (isExpanded) {
            setIsExpanded(false)
            setExpandedId("")
        } else {
            setIsExpanded(true);
            setExpandedId(id);
        }
    }

    useEffect(() => { 
        setIsSelected(selectedRecords.filter((x : OdsData) => x.organisationCode == node.organisationCode).length > 0)
    },[node, selectedRecords])

    useEffect(() => {
        selectedRecords.forEach(selectedRecord => {
            if(selectedRecord.odsHierarchy != node.odsHierarchy){
                if(selectedRecord.odsHierarchy.startsWith(node.odsHierarchy)){
                    const nodeRef: HTMLInputElement = document.getElementById(node.id) as HTMLInputElement
                    if (nodeRef) {
                        nodeRef.indeterminate = true;
                    }
                }
            }
        })
    }, [node, selectedRecords]);

    const processCheck = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.checked) {
            addSelectedRecord(node)
            setIsSelected(true);
        } else {
            removeSelectedRecord(node);
            setIsSelected(false);
        }
    }

    const processChildAdds = (organisationCode: OdsData) => {
        setSelectedChildren([...selectedChildren, organisationCode])
        addSelectedRecord(organisationCode)
    }

    const processChildRemoves = (organisation: OdsData) => {
        const next: OdsData[] = selectedChildren.filter((x : OdsData)=> x.organisationCode !== organisation.organisationCode);
        if (!next.length) {
            const nodeRef: HTMLInputElement = document.getElementById(node.id) as HTMLInputElement
            if (nodeRef) {
                nodeRef.indeterminate = false;
            }
        }
        setSelectedChildren(next);
        removeSelectedRecord(organisation)
    }

    return (<div style={{ paddingLeft: '30px' }}>
        <span style={{ width: "20px", display: "inline-block" }}>
            {node.hasChildren ?
                <span onClick={() => { toggle(node.id) }} >
                    {isExpanded ? <FontAwesomeIcon icon={faMinus} /> : <FontAwesomeIcon icon={faPlus} />}
                </span>
                :
                <></>
            }
        </span>

        <span>{node.organisationName}({node.organisationCode})</span>
        &nbsp;
        <span><Form.Check inline onChange={processCheck} disabled={parentSelected} checked={selected || parentSelected } id={`${node.id}`} /></span>
        {isLoading && <FontAwesomeIcon icon={faSpinner} pulse />}
        {data && data.map((element: OdsData) => <span key={`${node.id}:${element.id}`}>
            <OdsTreeElement 
                    selectedRecords={selectedRecords} 
                    node={element} 
                    addSelectedRecord={processChildAdds} 
                    removeSelectedRecord={processChildRemoves} 
                    parentSelected={parentSelected || selected }
                    ></OdsTreeElement>
        </span>)}
    </div>)
}
