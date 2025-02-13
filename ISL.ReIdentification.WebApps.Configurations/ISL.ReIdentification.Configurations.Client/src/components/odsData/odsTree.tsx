import { FunctionComponent, useEffect, useState, useRef } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { OdsTreeElement } from "./odsTreeElement";
import { toastError } from "../../brokers/toastBroker.error";
import { Form, InputGroup, Spinner } from "react-bootstrap";
import { Input } from "nhsuk-react-components";

type OdsTreeProps = {
    rootId: string;
    selectedRecords: OdsData[];
    setSelectedRecords: (selectedRecords: OdsData[]) => void;
    readonly: boolean;
    showRoot?: boolean
};

const OdsTree: FunctionComponent<OdsTreeProps> = ({ rootId, selectedRecords, setSelectedRecords, readonly = false, showRoot = false }) => {

    const { data: rootRecord, isLoading } = odsDataService.useRetrieveAllOdsData(`?filter=Id eq ${rootId}`)
    const { data } = odsDataService.useGetOdsChildren(rootId);
    const tree = useRef<HTMLSpanElement>(null);

    const addSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords.filter(o => o.organisationCode != odsRecord.organisationCode)])
    }

    useEffect(() => {
        if (!tree.current) {
            return
        }
        const indeterminate = tree.current.querySelectorAll<HTMLInputElement>('input:indeterminate');

        for (let i = 0; i < indeterminate.length; i++) {
            indeterminate[i].indeterminate = false;
            if (!indeterminate[i].dataset)
                continue;
            let path = indeterminate[i].dataset.odsHierarchy;
            if (!path) {
                continue;
            }

            if (selectedRecords.find(x => x.odsHierarchy.startsWith(path))) {
                indeterminate[i].indeterminate = true;
            } else {
                indeterminate[i].indeterminate = false;
            }
        }
    }, [selectedRecords]);

    if (isLoading || !rootRecord) {
        return <Spinner />
    }

    const rootSelected = (): boolean => {
        return selectedRecords.filter((x: OdsData) => x.organisationCode == rootRecord[0].organisationCode).length > 0
    }

    return (
        <div ref={tree}>
            {showRoot && <Form>
                <Form.Check data-ods-hierarchy={rootRecord[0].odsHierarchy} inline checked={rootSelected()} onChange={(e) => { e.target.checked ? addSelectedRecord(rootRecord[0]) : removeSelectedRecord(rootRecord[0]) }} />
                <span>{rootRecord[0].organisationName}({rootRecord[0].organisationCode})</span>
            </Form>
            }

            {data && data.map((element: OdsData) => {
                return <span key={element.id} >
                    <OdsTreeElement
                        node={element}
                        addSelectedRecord={addSelectedRecord}
                        removeSelectedRecord={removeSelectedRecord}
                        parentSelected={false}
                        selectedRecords={selectedRecords}
                        readonly={readonly}
                    ></OdsTreeElement>
                </span>;
            }
            )}
        </div>
    );
};

export default OdsTree;

