import { InteractionRequiredAuthError } from "@azure/msal-browser";
import { useMsal } from "@azure/msal-react";
import { faCopy, faLeftLong, faRightLong } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { PowerBIEmbed } from "powerbi-client-react";
import { useEffect, useState } from "react";
import { Button, ButtonGroup, Container, Dropdown, DropdownButton, Form, Navbar, Table, Toast, ToastContainer } from "react-bootstrap";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";
import { useParams } from "react-router-dom";
import { IReportEmbedConfiguration, service } from "powerbi-client";

type reIdRecord = {
    pseudo: string,
    nhsnumber: string
}

export const ReIdReport = () => {

    const { reportGroupId, reportId, psuedoColumn } = useParams();
    const [position, setPosition] = useState<ToastPosition>("bottom-end");
    const [toastHidden, setToastHidden] = useState(false);
    const { accounts, instance } = useMsal();
    const [lastSelectedRecord, setLastSelectedRecord] = useState("");
    const [reidentificationCache, setReidentificationCache] = useState<string[]>([]);
    const [reidentifications, setReIdentifications] = useState<reIdRecord[]>([]);
    const [reidReason, setReidReason] = useState("WOW");
    const [startedReid, setStartedReid] = useState(true);
    const [showDeveloperTools, setShowDeveloperTools] = useState("");
    const [reportConfig, setReportConfig] = useState<IReportEmbedConfiguration>();

    const embedReport = () => {
        const loadReport = async () => {
            if(reportConfig){
                return;
            }

            await instance.initialize();
            const activeAccount = instance.getActiveAccount();
            const accounts = instance.getAllAccounts();

            const request = {
                scopes: ["https://analysis.windows.net/powerbi/api/Report.Read.All"],
                account: activeAccount || accounts[0]
            };
            let authResult;
            try {
                authResult = await instance.acquireTokenSilent(request);
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    // fallback to interaction when silent call fails
                    await instance.acquireTokenRedirect(request);
                } else {
                    console.log(error);
                    throw error; // rethrow the error after logging it
                }
            }

            fetch("https://api.powerbi.com/v1.0/myorg/groups/" + reportGroupId + "/reports/" + reportId, {
                headers: {
                    "Authorization": "Bearer " + authResult?.accessToken
                },
                method: "GET"
            })
                .then(r => r.json()).then((d) => {
                    const config= {
                        type: 'report',
                        embedUrl: d.embedUrl,
                        accessToken: authResult?.accessToken
                    };
                    setReportConfig(config)
                });
        }
        loadReport();
    };

    useEffect(() => {
        embedReport();
    },[])

    useEffect(() => {
        if (reidReason) {
            reidentify(lastSelectedRecord);
        }
    }, [reidReason])

    function reidentify(p: string) {
        if(!p) 
            return;
        setLastSelectedRecord(p);
        if (!startedReid) {
            setStartedReid(true);
        }
        setReIdentifications((reidentifications) => {
            if(!reidentifications.find((ri) => ri.pseudo == p)){
                return [...reidentifications, { pseudo: p, nhsnumber: "foo" } as reIdRecord]
            }
            return reidentifications;
        });
    }

    const ReportComponent = () => <><PowerBIEmbed 
        embedConfig={reportConfig as IReportEmbedConfiguration} 
        cssClassName="report-container" 
        getEmbeddedComponent={(embedObject) => {
            embedObject.on('loaded', () => { 
                console.log('Loaded binding events');
                embedObject.on('dataSelected', (event?: CustomEvent) => {
                    console.log(event);
                    if (event && event.detail.dataPoints[0]) {
                        const pseudoValue = event.detail.dataPoints[0].identity.find((x: any) => x.target.column.toLowerCase() == psuedoColumn?.toLowerCase())
                        reidentify("" + pseudoValue.equals);
                    }
                })
             });
           
        }}
    /></>

    return (
        <>
            <Container className="min-vh-100 d-flex flex-column p-0 m-0" fluid>
                <Navbar style={{ padding: 0 }}>
                    <Container fluid>
                        <Navbar.Brand style={{ fontSize: "1em", padding: 0 }}>
                            ISL Reidentification Portal
                        </Navbar.Brand>

                        {toastHidden && <Button size="sm" onClick={() => { setToastHidden(false) }}>Unhide Reidetification popup</Button>}
                        <Navbar.Text style={{ padding: 1 }}>
                            <DropdownButton title={accounts[0].username} size="sm">
                                <Dropdown.Item>Logout</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Header>Position Re-id</Dropdown.Header>
                                <Dropdown.Item onClick={() => { setPosition("top-start") }}>Top-Left</Dropdown.Item>
                                <Dropdown.Item onClick={() => { setPosition("top-end") }}>Top-Right</Dropdown.Item>
                                <Dropdown.Item onClick={() => { setPosition("bottom-start") }}>Bottom-Right</Dropdown.Item>
                                <Dropdown.Item onClick={() => { setPosition("bottom-end") }}>Bottom-Left</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item onClick={() => { setShowDeveloperTools("end"); }}>Show Developer Tools</Dropdown.Item>
                            </DropdownButton>
                        </Navbar.Text>
                    </Container>
                </Navbar>
                <div className="flex-grow-1" style={{ background: "pink", position:"absolute", width:"100%", height:visualViewport?.height - 40, inset: "33px 0 0 0" }}>
                    {
                        reportConfig && <>{ReportComponent()}</>
                    }
                </div>
            </Container>
            {startedReid &&
                <ToastContainer position={position} hidden={toastHidden}>
                    <Toast onClose={() => { setToastHidden(true) }}>
                        <Toast.Header>
                            <strong className="me-auto">Reidentification</strong>
                            <Button size="sm" onClick={() => { setReIdentifications([]); setReidentificationCache([]); }}>Clear</Button>
                        </Toast.Header>
                        <Toast.Body>
                            {!reidReason ?
                                <>
                                    <div>You have selected {reidentificationCache.length} patients.</div>
                                    <div>Why are you re-identifying these patients?</div>
                                    <Form.Select onChange={(e) => { setReidReason(e.target.value) }}>
                                        <option value="">--- Select Reason ---</option>
                                        <option value="foo">foo!</option>
                                        <option value="bar">bar!</option>
                                    </Form.Select>
                                </>
                                :
                                <Table size="sm" bordered>
                                    <tbody>
                                        {reidentifications.map((ri) => <tr className={ri.pseudo === lastSelectedRecord ? "table-active" : ""}>
                                            <td>{ri.pseudo}</td>
                                            <td>{ri.nhsnumber}</td>
                                            <td><FontAwesomeIcon icon={faCopy} /></td>
                                        </tr>)}
                                    </tbody>
                                </Table>
                            }
                        </Toast.Body>
                    </Toast>
                </ToastContainer>
            }
            {showDeveloperTools &&
                <div className={`offcanvas offcanvas-${showDeveloperTools} show`} >
                    <ButtonGroup size="sm">
                        <Button onClick={() => { setShowDeveloperTools("start") }}><FontAwesomeIcon icon={faLeftLong} /></Button>
                        <Button onClick={() => { setShowDeveloperTools("end") }}><FontAwesomeIcon icon={faRightLong} /></Button>
                        <Button onClick={() => { setShowDeveloperTools("") }}>Close</Button>
                    </ButtonGroup>
                </div>
            }
        </>
    )
}

