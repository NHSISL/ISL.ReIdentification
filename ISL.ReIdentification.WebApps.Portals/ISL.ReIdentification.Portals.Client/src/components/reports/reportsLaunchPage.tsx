import { IReportEmbedConfiguration } from "powerbi-client";
import { PowerBIEmbed } from "powerbi-client-react";
import { FunctionComponent, useEffect, useState } from "react";
import ReportToast from "./reportToast";
import { ReIdRecord } from "../../types/ReIdRecord";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { useParams } from "react-router-dom";
import { PBIEvent } from "../../types/PBIEvent";
import { reIdentificationService } from "../../services/foundations/reIdentificationService";
import { AccessRequest } from "../../models/accessRequest/accessRequest";
import { useMsal } from "@azure/msal-react";

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
    const { submit } = reIdentificationService.useRequestReIdentification();

    const { accounts } = useMsal();

    const reid = async (record: string) => {
        setReidentifications((reidentifications) => {
            setLastSelectedPseudo(record);
            if (reidentifications.findIndex(x => x.pseudo === record) === -1) {
                return [...reidentifications, { pseudo: record, nhsnumber: "XX", loading: true }];
            }
            return reidentifications;
        })
    }

    useEffect(() => {
        async function x() {
            if (reidentifications.length) {
                const toCallRecords = reidentifications.filter(x => !x.hasCalled);
                if (toCallRecords) {
                    toCallRecords.forEach(async x => {
                        setReidentifications((reidentifications) => {
                            const next = [...reidentifications];
                            const v = next.filter(y => y.pseudo === x.pseudo);
                            v[0].hasCalled = true;
                            return next
                        })

                        const acc = accounts[0];
                        const identificationRequest: AccessRequest = {
                            csvIdentificationRequest: undefined,
                            impersonationContext: undefined,
                            identificationRequest: {
                                id: crypto.randomUUID(),
                                identificationItems: [{
                                    rowNumber: "1",
                                    identifier: x.pseudo,
                                    hasAccess: undefined,
                                    message: undefined,
                                    isReidentified: undefined,
                                }],
                                DisplayName: acc.name || "",
                                GivenName: "TODO",
                                email: acc.username,
                                JobTitle: "TODO",
                                Organisation: "TODO",
                                Surname: "TODO",
                                reason: "WOW"
                            }
                        }

                        const r = await submit(identificationRequest)

                        setReidentifications((reidentifications) => {
                            const next = [...reidentifications];
                            const v = next.filter(y => y.pseudo === x.pseudo);
                            v[0].nhsnumber = r.identificationRequest?.identificationItems[0].identifier;
                            v[0].loading = false;
                            return next
                        })
                    });
                }
            }
        }

        x();

    }, [reidentifications, setReidentifications, accounts, submit])



    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const bindEvents = (embedObject: any) => {
        embedObject.on('loaded', () => {
            addDeveloperEvent({
                message: "bound events",
            })
            embedObject.on('dataSelected', (event?: CustomEvent<PBIEvent>) => {
                addDeveloperEvent({
                    message: "dataSelected",
                });
                if (event && event.detail.dataPoints[0]) {
                    const pseudo = event.detail.dataPoints[0].identity.filter((x) => x.target.column.toLowerCase() == pseudoColumn?.toLowerCase());
                    if (pseudo.length !== 1) {
                        addDeveloperEvent({
                            message: `dataSelected: pseudo column (${pseudoColumn}) not found.`,
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


