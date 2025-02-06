import { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { OdsTreeElement } from "./odsTreeElement";
import { toastError } from "../../brokers/toastBroker.error";
import { Spinner } from "react-bootstrap";

type OdsTreeProps = {
    rootId: string;
    selectedRecords: OdsData[];
    setSelectedRecords: (selectedRecords: OdsData[]) => void;
    readonly: boolean;
    showRoot?: boolean
};

const OdsTree: FunctionComponent<OdsTreeProps> = ({ rootId, selectedRecords, setSelectedRecords, readonly = false, showRoot = false }) => {

    const { data: rootRecord, isLoading  } = odsDataService.useRetrieveAllOdsData(`?filter=Id eq ${rootId}`)
    const { data } = odsDataService.useGetOdsChildren(rootId);

    const addSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords.filter(o => o.organisationCode != odsRecord.organisationCode)])
    }

    useEffect(() => {
        console.log("SR");

        const x = document.querySelectorAll('input:indeterminate');

        for (let i = 0; i < x.length; i++) {
            x[i].indeterminate = false;
            if (!x[i].dataset)
                return
            console.log(selectedRecords);
            let path = x[i].dataset.fooBar;
            console.log(path);
            if (selectedRecords.find(x => x.odsHierarchy.startsWith(path))) {
                x[i].indeterminate = true;
            } else {
                x[i].indeterminate = false;
            }
        }





    }, [selectedRecords]);

    if (isLoading || !rootRecord)
    {
        return <Spinner />
    }

    return (
        <>
            {showRoot && <> { rootRecord[0].organisationName }({ rootRecord[0].organisationCode }) </>}
            {data && data.map((element: OdsData) => {
                return <span key={element.id}>
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
        </>
    );
};

export default OdsTree;