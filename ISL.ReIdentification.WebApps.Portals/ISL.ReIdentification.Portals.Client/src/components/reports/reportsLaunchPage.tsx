import { IReportEmbedConfiguration } from "powerbi-client";
import { PowerBIEmbed } from "powerbi-client-react";
import { FunctionComponent, useState } from "react";
import ReportToast from "./reportToast";
import { ReIdRecord } from "../../types/ReIdRecord";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { useParams } from "react-router-dom";
import { PBIEvent } from "../../types/PBIEvent";

type ReportLaunchPageProps = {
    reportConfig: IReportEmbedConfiguration
    addDeveloperEvent: (event: DeveloperEvents) => void;
    toastPostion: ToastPosition;
}



const ReportsLaunchPage: FunctionComponent<ReportLaunchPageProps> = (props) => {

    const { reportConfig, addDeveloperEvent, toastPostion } = props;
    const { pseudoColumn } = useParams();
    const [reidentifications, setReidentifications] = useState<ReIdRecord[]>([]);
    const [toastHidden, setToastHidden] = useState(false);
    const [lastSelectedPseudo, setLastSelectedPseudo] = useState<string>("");

    const updatePseudoRecord = (record:string) => {
        setReidentifications(reidentifications => {
            const next = [...reidentifications];
            const i = next.find((x:ReIdRecord) =>  x.pseudo === record);
            if(i) {
                i.loading = false;
            }
            return next
        })
    }

    const reid = (record: string) => {
        setReidentifications((reidentifications) => {
            setLastSelectedPseudo(record);
            if(reidentifications.findIndex(x => x.pseudo === record) === -1) {
                setTimeout(() => {
                    updatePseudoRecord(record);
                }, 500);
                return [...reidentifications, {pseudo: record, nhsnumber:"foo", loading:true  }];
            }
            return reidentifications;
        })
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const bindEvents = (embedObject: any) => {
        embedObject.on('loaded', () => {
            addDeveloperEvent({
                message: "bound events",
            })
            embedObject.on('dataSelected', (event?: CustomEvent<PBIEvent>) => {
                console.log(event);
                addDeveloperEvent({
                    message: "dataSelected",
                });
                if (event && event.detail.dataPoints[0]) {
                    const pseudo = event.detail.dataPoints[0].identity.filter((x) => x.target.column.toLowerCase() == pseudoColumn?.toLowerCase());
                    if (pseudo.length !== 1) {
                        addDeveloperEvent({
                            message: `dataSelected: pseudo (${pseudo}) columns does not equal 1 fount ${pseudo.length}`,
                            eventDetails: event
                        });
                        return;
                    }
                    addDeveloperEvent({
                        message: `dataSelected: pseudo (${pseudo[0].equals}) found`
                    })
                    reid("" + pseudo[0].equals);
                } else {
                    addDeveloperEvent({
                        message: "dataSelected: no datapoints found",
                        eventDetails: event
                    });
                }
            })
        })
    }

    return <><div className="flex-grow-1" style={{ background: "pink", position: "absolute", width: "100%", height: (visualViewport ? visualViewport.height : 500) - 40, inset: "33px 0 0 0" }}>
        <PowerBIEmbed
            embedConfig={reportConfig as IReportEmbedConfiguration}
            cssClassName="report-container"
            getEmbeddedComponent={bindEvents}
        />
        <ReportToast
            clearList={() => { setReidentifications([]) }}
            hidden={toastHidden} hide={() => { setToastHidden(true) }}
            lastSelectedPseudo={lastSelectedPseudo}
            position={toastPostion}
            reidentifications={reidentifications} />
    </div>
    </>
}

export default ReportsLaunchPage


