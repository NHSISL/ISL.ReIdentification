import { FunctionComponent, useEffect, useState } from "react";
import { Button, Card, Col, Container, Dropdown, DropdownButton, Navbar, Row } from "react-bootstrap";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import ReportsReasonPage from "./ReportsReasonPage";
import ReportsLaunchPage from "./reportsLaunchPage";
import axios, { AxiosError } from "axios";
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
    const [noAccess, setNoAccess] = useState(false);

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

    const aquireReportEmbeddingUrl = async (accessToken: string) => {
        return axios.get("https://api.powerbi.com/v1.0/myorg/groups/" + "68fc89a2-5471-452c-bee0-d37a0b3cd99f" + "/reports/" + "df06f0a1-f4f1-44d4-b94c-cdbda9de5bad",
            {
                headers: {
                    "Authorization": "Bearer " + accessToken
                }
            }
        ).then(response => {
            return response.data.embedUrl;
        });
    }

    const launch = async () => {

        const at = await aquireAccessToken();
        if (at) {
            try {
                const reportUrl = await aquireReportEmbeddingUrl(at);
                setReportConfig({
                    type: 'report',
                    embedUrl: reportUrl,
                    accessToken: at
                } as IReportEmbedConfiguration);

                setIsLaunched(true);
            } catch (err) {
                const error = err as Error | AxiosError;
                if (axios.isAxiosError(error)) {
                    if(error.response && error.response.status === 401) {
                        setNoAccess(true);
                        return;
                    }
                }
                throw err;
            }
        }
    }

    useEffect(() => {
        if (lastEvent && showDeveloperTools) {
            setDeveloperEvents((developerEvents) => [...developerEvents, lastEvent]);
        }
    }, [showDeveloperTools, lastEvent])

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
                    <UnauthenticatedTemplate>
                        <Card>
                            <Card.Body>
                                <p>Please Login to use the re-identfication Portal</p>
                                <p><Button onClick={() => { instance.loginPopup() }}>Login</Button></p>
                            </Card.Body>
                        </Card>
                    </UnauthenticatedTemplate>
                    <AuthenticatedTemplate>
                        {!noAccess && !isLaunched && <ReportsReasonPage launchReport={launch} reidReason={reidReason} setReidReason={setReidReason} />}
                        {noAccess && accounts.length && <Card>
                            <Card.Header>No Access </Card.Header>
                            <Card.Body>
                                <p>You do not have access to this report with the account:</p>
                                <p>{accounts[0].username} ({accounts[0].name}).</p>
                                <p>Please contact your local service desk for access.</p>
                                <Button onClick={() => { instance.logout(); setNoAccess(true); }}>Logout</Button>
                            </Card.Body>
                        </Card>}
                        {isLaunched && reportConfig &&
                            <ReportsLaunchPage
                                reportConfig={reportConfig}
                                addDeveloperEvent={raiseEvent}
                                toastPostion={toastPostion} />
                        }
                    </AuthenticatedTemplate>
                </Col>
            </Row>
            <ReportDeveloperTools
                developerToolsLocation={showDeveloperTools}
                setDeveloperToolsLocation={setShowDeveloperTools}
                eventsList={developerEvents} />
        </Container>
    )
}

export default ReportsHome