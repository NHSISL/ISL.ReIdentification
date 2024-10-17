import React, { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { Form } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinus, faPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { toast } from "react-toastify";
import { toastError, toastSuccess } from "../../brokers/toastBroker";

type OdsTreeProps = {
    rootName: string;
    selectedRecords: string[];
    setSelectedRecords: (selectedRecords: string[]) => void;
};

type OdsTreeElementProps = {
    node: OdsData,
    addSelectedRecord: (orgCode: string) => void,
    removeSelectedRecord: (orgCode: string) => void,
    parentSelected: boolean
}

const OdsTreeElement: FunctionComponent<OdsTreeElementProps> = ({ node, addSelectedRecord, removeSelectedRecord, parentSelected }) => {
    const [isExpanded, setIsExpanded] = useState(false);
    const [expandedId, setExpandedId] = useState("");
    const { data, isLoading } = odsDataService.useGetOdsChildren(expandedId);
    const [selected, setIsSelected] = useState(false);
    const [selectedChildren, setSelectedChildren] = useState<string[]>([]);

    const toggle = (id: string) => {
        if (isExpanded) {
            setIsExpanded(false)
            setExpandedId("")
        } else {
            setIsExpanded(true);
            setExpandedId(id);
        }
    }


    const processCheck = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.checked) {
            addSelectedRecord(node.organisationCode)
            setIsSelected(true);
        } else {
            removeSelectedRecord(node.organisationCode);
            setIsSelected(false);
        }
    }

    const processChildAdds = (organisationCode: string) => {
        const nodeRef: HTMLInputElement = document.getElementById(node.id) as HTMLInputElement
        if (nodeRef) {
            nodeRef.indeterminate = true;
        }
        setSelectedChildren([...selectedChildren, organisationCode])
        addSelectedRecord(organisationCode)
    }

    const processChildRemoves = (organisationCode: string) => {
        const next: string[] = selectedChildren.filter(x => x !== organisationCode);

        if (!next.length) {
            const nodeRef: HTMLInputElement = document.getElementById(node.id) as HTMLInputElement
            if (nodeRef) {
                nodeRef.indeterminate = false;
            }
        }
        setSelectedChildren(next);
        removeSelectedRecord(organisationCode)
        console.log(node.organisationCode);
        console.log(next);
    }

    return (<div style={{ paddingLeft: '30px' }}>
        <span style={{ width: "20px", display: "inline-block" }}>
            {node.hasChildren ?
                <span onClick={() => { toggle(node.id) }} >
                    {isExpanded ? <FontAwesomeIcon icon={faMinus} /> : <FontAwesomeIcon icon={faPlus} />}
                </span>
                :
                ""
            }
        </span>

        <span>{node.organisationCode}</span>
        &nbsp;
        <span><Form.Check inline onChange={processCheck} disabled={parentSelected} checked={selected || parentSelected} id={`${node.id}`} /></span>
        {isLoading && <FontAwesomeIcon icon={faSpinner} pulse />}
        {data && data.map((element: OdsData) => <span key={`${node.id}:${element.id}`}>
            <OdsTreeElement node={element} addSelectedRecord={processChildAdds} removeSelectedRecord={processChildRemoves} parentSelected={parentSelected || selected}></OdsTreeElement>
        </span>)}
    </div>)
}


const OdsTree: FunctionComponent<OdsTreeProps> = ({ rootName, selectedRecords, setSelectedRecords }) => {

    const { data: rootRecord } = odsDataService.useRetrieveAllOdsData(`?filter=OrganisationCode eq '${rootName}'`)
    const [rootRecordId, setRootRecordId] = useState("");
    const { data } = odsDataService.useGetOdsChildren(rootRecordId);
    const [isSelected, setIsSelected] = useState(false);

    useEffect(() => {
        if (!rootRecord) {
            return;
        };

        if (rootRecord.length != 1) {
            console.log("Did not receive one Root:");
            toastError("Did not receive one Root:");
            return;
        }

        setRootRecordId(rootRecord[0].id);

    }, [rootRecord])

    const addSelectedRecord = (odsRecord: string) => {
        setSelectedRecords([...selectedRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: string) => {
        setSelectedRecords([...selectedRecords.filter(o => o != odsRecord)])
    }

    return (
        <>
            {data && data.map((element: OdsData) => <span key={element.id} >
                <OdsTreeElement
                    node={element}
                    addSelectedRecord={addSelectedRecord}
                    removeSelectedRecord={removeSelectedRecord}
                    parentSelected={isSelected}
                ></OdsTreeElement>
            </span>
            )}
        </>
    );
};

export default OdsTree;