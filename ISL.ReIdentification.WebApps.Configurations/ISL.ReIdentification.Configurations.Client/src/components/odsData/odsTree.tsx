import { FunctionComponent, useEffect, useState } from "react";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { OdsData } from "../../models/odsData/odsData";
import { toastError } from "../../brokers/toastBroker";
import { OdsTreeElement } from "./odsTreeElement";

type OdsTreeProps = {
    rootName: string;
    selectedRecords: OdsData[];
    setSelectedRecords: (selectedRecords: OdsData[]) => void;
};

const OdsTree: FunctionComponent<OdsTreeProps> = ({ rootName, selectedRecords, setSelectedRecords }) => {

    const { data: rootRecord } = odsDataService.useRetrieveAllOdsData(`?filter=OrganisationCode eq '${rootName}'`)
    const [rootRecordId, setRootRecordId] = useState("");
    const { data } = odsDataService.useGetOdsChildren(rootRecordId);

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

    const addSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords, odsRecord]);
    }

    const removeSelectedRecord = (odsRecord: OdsData) => {
        setSelectedRecords([...selectedRecords.filter(o => o.organisationCode != odsRecord.organisationCode)])
    }

    return (
        <>
            {data && data.map((element: OdsData) => {
                return <span key={element.id}>
                    <OdsTreeElement
                        node={element}
                        addSelectedRecord={addSelectedRecord}
                        removeSelectedRecord={removeSelectedRecord}
                        parentSelected={false}
                        selectedRecords={selectedRecords}
                    ></OdsTreeElement>
                </span>;
            }
            )}
        </>
    );
};

export default OdsTree;