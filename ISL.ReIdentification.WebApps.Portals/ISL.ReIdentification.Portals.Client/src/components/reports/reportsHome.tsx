import { FunctionComponent, useEffect, useState } from "react";
import { Col, Container, Dropdown, DropdownButton, Navbar, Row } from "react-bootstrap";
import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";
import ReportsReasonPage from "./ReportsReasonPage";
import ReportsLaunchPage from "./reportsLaunchPage";
import axios from "axios";
import { InteractionRequiredAuthError } from "@azure/msal-browser";
import { IReportEmbedConfiguration } from "embed";
import ReportDeveloperTools from "./reportDeveloperTools";
import { DeveloperEvents } from "../../types/DeveloperEvents";
import { ToastPosition } from "react-bootstrap/esm/ToastContainer";

const ReportsHome: FunctionComponent = () => {
    const { accounts, instance } = useMsal();
    const [toastPostion, setToastPosition] = useState<ToastPosition>("bottom-end")
    const [showDeveloperTools, setShowDeveloperTools] = useState("");
    const [reidReason, setReidReason] = useState("");
    const [isLaunched, setIsLaunched] = useState(false);
    const [reportConfig, setReportConfig] = useState<IReportEmbedConfiguration>();
    const [developerEvents, setDeveloperEvents] = useState<DeveloperEvents[]>([]);
    const [lastEvent, setLastEvent] = useState<DeveloperEvents>();
    

    const aquireAccessToken = async () => {
        await instance.initialize();
            const activeAccount = instance.getActiveAccount();
            const accounts = instance.getAllAccounts();

            const request = {
                scopes: ["https://analysis.windows.net/powerbi/api/Report.Read.All"],
                account: activeAccount || accounts[0]
            };
            try {
                return (await instance.acquireTokenSilent(request)).accessToken;
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    // fallback to interaction when silent call fails
                    await instance.acquireTokenRedirect(request);
                } else {
                    console.log(error);
                    throw error; // rethrow the error after logging it
                }
            }
    }

    const aquireReportEmbeddingUrl = async ( accessToken : string) =>{
        const response = await axios.get("https://api.powerbi.com/v1.0/myorg/groups/" + "68fc89a2-5471-452c-bee0-d37a0b3cd99f" + "/reports/" + "df06f0a1-f4f1-44d4-b94c-cdbda9de5bad",
            { 
                headers: {
                    "Authorization": "Bearer " + accessToken
                }
            }
        )
        return response.data.embedUrl;
    }

    const launch = async () => {
        
        const at = await aquireAccessToken();
        if(at){
            const reportUrl = await aquireReportEmbeddingUrl(at);
            setReportConfig({
                    type: 'report',
                    embedUrl: reportUrl,
                    accessToken: at
            } as IReportEmbedConfiguration);

            setIsLaunched(true);
        }
    }

    useEffect(() => {
        if(lastEvent && showDeveloperTools) {
            setDeveloperEvents((developerEvents) => [...developerEvents, lastEvent]);
        }
    },[showDeveloperTools, lastEvent])

    const raiseEvent = (event: DeveloperEvents) => {
        setLastEvent(event)
    }

    return (
        <Container className="min-vh-100 d-flex flex-column p-0 m-0" fluid>
            <Row className="m-0">
                <Navbar style={{ padding: 1 }}>
                    <Container fluid>
                        <Navbar.Brand style={{ fontSize: "1em", padding: 0 }}>
                            ISL Reidentification Portal
                        </Navbar.Brand>
                        {accounts.length > 0 &&
                            <AuthenticatedTemplate>
                                <Navbar.Text style={{ padding: 1 }}>
                                    <DropdownButton title={accounts[0].username} size="sm">
                                        <Dropdown.Item onClick={() => instance.logout()}>Logout</Dropdown.Item>
                                        <Dropdown.Divider />
                                        <Dropdown.Header>Position Re-id</Dropdown.Header>
                                        <Dropdown.Item onClick={() => { setToastPosition("top-start") }}>Top-Left</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("top-end") }}>Top-Right</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("bottom-start") }}>Bottom-Right</Dropdown.Item>
                                        <Dropdown.Item onClick={() => { setToastPosition("bottom-end") }}>Bottom-Left</Dropdown.Item>
                                        <Dropdown.Divider />
                                        <Dropdown.Item onClick={() => { setShowDeveloperTools("end"); }}>Show Developer Tools</Dropdown.Item>
                                    </DropdownButton>
                                </Navbar.Text>
                            </AuthenticatedTemplate>
                        }
                    </Container>
                </Navbar>
            </Row>
           <Row className="flex-grow-1 align-items-center m-0">
                <Col>
                        {isLaunched ? 
                            reportConfig && 
                                <ReportsLaunchPage 
                                    reportConfig={reportConfig}
                                    addDeveloperEvent={raiseEvent} 
                                    toastPostion={toastPostion}/> 
                        : 
                            <ReportsReasonPage launchReport={launch} reidReason={reidReason} setReidReason={setReidReason}  />
                        }
                </Col>
            </Row>
            <ReportDeveloperTools 
                developerToolsLocation={showDeveloperTools} 
                setDeveloperToolsLocation={setShowDeveloperTools}
                eventsList={developerEvents}/>
        </Container>
    )
}

export default ReportsHome