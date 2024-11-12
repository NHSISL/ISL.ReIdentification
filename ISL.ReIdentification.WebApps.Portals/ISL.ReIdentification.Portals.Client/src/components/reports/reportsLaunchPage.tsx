import { IReportEmbedConfiguration } from "powerbi-client";
import { PowerBIEmbed } from "powerbi-client-react";
import { FunctionComponent, useState } from "react";
import ReportToast from "./reportToast";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { useParams } from "react-router-dom";
import { PBIEvent, PBIIdentity, PBIValues } from "../../types/PBIEvent";
import { useReidentification } from "../../hooks/useReidentification";
import { Button, ButtonGroup, Modal } from "react-bootstrap";

type ReportLaunchPageProps = {
    reportConfig: IReportEmbedConfiguration
    addDeveloperEvent: (event: DeveloperEvents) => void;
    toastPostion: ToastPosition;
    activePage?: string,
    toastHidden: boolean,
    hideToast: () => void,
    reidReason: string
}

const ReportsLaunchPage: FunctionComponent<ReportLaunchPageProps> = (props) => {
    const { reportConfig, addDeveloperEvent, toastPostion, activePage, toastHidden, hideToast, reidReason } = props;
    const { pseudoColumn } = useParams();
    const [heldPseudosToReid, setHeldPseudosToReid ] = useState<string[]>([]);
    const [lastSetOfPseudos, setLastSetOfPseudos] = useState<string[]>([]);
    const [promptForReid, setPromptForReid] = useState(false);
    const { reidentify, reidentifications, lastPseudo, clearList, isLoading } = useReidentification(reidReason);

    const reIdBulk = () => {
        setPromptForReid(false)
        setLastSetOfPseudos(heldPseudosToReid);
        reidentify(heldPseudosToReid);
        setHeldPseudosToReid([]);
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const bindEvents = (embedObject: any) => {
        embedObject.on('loaded', async () => {
            addDeveloperEvent({ message: "bound events" })

            if (activePage) {
                const pages = await embedObject.getPages();
                const pageToActivate = pages.filter((page: { displayName: string; }) => page.displayName === activePage)
                if (pageToActivate.length !== 1) {
                    addDeveloperEvent({ message: `Cannot find page: ${activePage}`, eventDetails: { detail: { availablePages: pages.map((p: { displayName: string; }) => p.displayName) } } })
                    return;
                }

                embedObject.setPage(pageToActivate[0].name);
            }

            embedObject.on('dataSelected', async (event?: CustomEvent<PBIEvent>) => {
                addDeveloperEvent({ message: "dataSelected" });
                if (event && event.detail.dataPoints[0]) {
                    // pseudo data could be held in either an identity or value field.
                    //const identityValues = ...event.detail.dataPoints.flatMap(x => x.identity);
                    const dataFields = [...event.detail.dataPoints.flatMap(x => x.identity), ...event.detail.dataPoints.flatMap(x => x.values)]
                    let pseudos: Array<PBIIdentity | PBIValues> = []

                    // report developer can provide multiple fields containing pseudo identifiers, so check all columns.
                    pseudoColumn?.split(':').forEach(pseudo => {
                        pseudos = [...pseudos, ...dataFields.filter((x) => {
                            // column could be either a column or a measure so check both properies.
                            if (x.target.column) {
                                return x.target.column.toLowerCase() == pseudo.toLowerCase()
                            }

                            if (x.target.measure) {
                                return x.target.measure.toLowerCase() == pseudo.toLowerCase()
                            }

                            return false;
                        })]
                    });

                    // no Pseduos selected so can return early.
                    if (!pseudos.length) {
                        return
                    }

                    //the psuedo could be contained in one of the following properties - so normalise all as strings
                    const normalisedPseudos = pseudos.map(x => {
                        if ((x as PBIIdentity).equals) {
                            return ("" + (x as PBIIdentity).equals)
                        }
                        return (x as PBIValues).value || (x as PBIValues).formattedValue;
                    });

                    // somevalues might be commaseperated lists of pseudos so we need to check and extract them to new items/
                    let formattedPseudos: string[] = [];
                    normalisedPseudos.forEach(maybeCommaSeperated => {
                        formattedPseudos = formattedPseudos.concat(maybeCommaSeperated.split(','))
                    })

                    // we might have been given a set containing duplicates so just remove all but one record;
                    const uniquePseudos = formattedPseudos.filter((value, index, array) => array.indexOf(value) === index);

                    //setLastSetOfPseudos(uniquePseudos);

                    // more than 10 so we prompt the user asking if they intended to re-id large number:
                    if (uniquePseudos.length > 5) {
                        //cache them and ask the question.
                        setHeldPseudosToReid(uniquePseudos);
                        setPromptForReid(true)
                    } else {
                        setLastSetOfPseudos(uniquePseudos);
                        await reidentify(uniquePseudos);
                    }
                } else {
                    addDeveloperEvent({ message: "dataSelected: no datapoints found", eventDetails: event });
                }
            })
        })
    }


    return <>
        <div className="flex-grow-1" style={{ position: "absolute", width: "100%", height: (visualViewport ? visualViewport.height : 500) - 40, inset: "33px 0 0 0" }}>
            <PowerBIEmbed
                embedConfig={reportConfig as IReportEmbedConfiguration}
                cssClassName="report-container"
                getEmbeddedComponent={bindEvents}
            />
            <Modal show={promptForReid}>
                <Modal.Header>Large Numbers of reid requests Detected</Modal.Header>
                <Modal.Body>
                    <p>You have requested {heldPseudosToReid.length} records to be re-identified.</p>
                    <p>This is likly to cause a breach to be reported.</p>
                    <ButtonGroup>
                        <Button onClick={reIdBulk}>Confirm</Button><Button variant="secondary" onClick={() => { setHeldPseudosToReid([]); setPromptForReid(false); setLastSetOfPseudos([]); }}>Cancel</Button>
                    </ButtonGroup>
                </Modal.Body>
            </Modal>
            <ReportToast
                clearList={clearList}
                hidden={toastHidden} hide={hideToast}
                lastSelectedPseudo={lastPseudo}
                lastPseudos={lastSetOfPseudos}
                recordLoading={isLoading}
                position={toastPostion}
                reidentifications={reidentifications} />
        </div>
    </>
}

export default ReportsLaunchPage


