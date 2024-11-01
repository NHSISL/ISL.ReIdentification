import { IReportEmbedConfiguration } from "powerbi-client";
import { PowerBIEmbed } from "powerbi-client-react";
import { FunctionComponent, useState } from "react";
import ReportToast from "./reportToast";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { useParams } from "react-router-dom";
import { PBIEvent } from "../../types/PBIEvent";
import { useReidentification } from "../../hooks/useReidentification";

type ReportLaunchPageProps = {
    reportConfig: IReportEmbedConfiguration
    addDeveloperEvent: (event: DeveloperEvents) => void;
    toastPostion: ToastPosition;
}

const ReportsLaunchPage: FunctionComponent<ReportLaunchPageProps> = (props) => {
    const { reportConfig, addDeveloperEvent, toastPostion } = props;
    const { pseudoColumn } = useParams();
    const [toastHidden, setToastHidden] = useState(false);
    const { reidentify, reidentifications, lastPseudo, clearList } = useReidentification("TODO");

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const bindEvents = (embedObject: any) => {
        embedObject.on('loaded', () => {
            addDeveloperEvent({ message: "bound events" })
            embedObject.on('dataSelected', (event?: CustomEvent<PBIEvent>) => {
                addDeveloperEvent({ message: "dataSelected" });
                if (event && event.detail.dataPoints[0]) {
                    const pseudo = event.detail.dataPoints[0].identity.filter((x) => x.target.column.toLowerCase() == pseudoColumn?.toLowerCase());
                    if (pseudo.length !== 1) {
                        addDeveloperEvent({ message: `dataSelected: pseudo column (${pseudoColumn}) not found.`, eventDetails: event });
                        return;
                    }
                    addDeveloperEvent({ message: `dataSelected: pseudo (${pseudo[0].equals}) found` })
                    reidentify("" + pseudo[0].equals);
                } else {
                    addDeveloperEvent({ message: "dataSelected: no datapoints found", eventDetails: event });
                }
            })
        })
    }

    return <>
        <div className="flex-grow-1" style={{ background: "pink", position: "absolute", width: "100%", height: (visualViewport ? visualViewport.height : 500) - 40, inset: "33px 0 0 0" }}>
            <PowerBIEmbed
                embedConfig={reportConfig as IReportEmbedConfiguration}
                cssClassName="report-container"
                getEmbeddedComponent={bindEvents}
            />
            <ReportToast
                clearList={clearList}
                hidden={toastHidden} hide={() => { setToastHidden(true) }}
                lastSelectedPseudo={lastPseudo}
                position={toastPostion}
                reidentifications={reidentifications} />
        </div>
    </>
}

export default ReportsLaunchPage


