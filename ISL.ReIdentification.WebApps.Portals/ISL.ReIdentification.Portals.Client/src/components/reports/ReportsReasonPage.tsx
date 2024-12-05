import { FunctionComponent, useState } from "react";
import { Button, Card, Container, Form, FormGroup, Spinner } from "react-bootstrap";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";
import { BreachModal } from "../breachDetails/BreachModal";

type ReportReasonPageProps = {
    setReidReason: (reason: string) => void;
    launchReport: () => void;
    reidReason: string;
};

const ReportsReasonPage: FunctionComponent<ReportReasonPageProps> = (props: ReportReasonPageProps) => {
    const { setReidReason, launchReport, reidReason } = props
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("", "Reasons");
    const [showBreachModal, setShowBreachModal] = useState(false);

    return <><Container>
        <Card>
            <Card.Header>
                <Card.Link href="/" style={{ color: "black", textDecoration: "none" }}>ISL Re-identification Portal</Card.Link>
            </Card.Header>
            <Card.Body>
                <div>
                    <h4>Welcome to the London Data Service Re-identification Portal</h4>
                    <p>The link you have followed provides re-identification integrated into Power BI reports.</p>
                    <p>The <a href="/">reidentification home page</a> has additional re-identification capabilities.</p>
                </div>
                <Form className="mt-5">
                    <FormGroup>
                        <Form.Label>Please select the reason why you are identifying the patients on this report:</Form.Label>
                        {isLoading ? <div><Spinner /></div> :
                            <Form.Select onChange={(e) => { setReidReason(e.target.value) }}>
                                <option value="">--- Select Reason ---</option>
                                {mappedLookups.map(l => <option key={l.id} value={l.name}>{l.name}</option>)}
                            </Form.Select>
                        }
                    </FormGroup>
                    <br />
                    <Button onClick={launchReport} disabled={!reidReason}>Launch Report</Button>
                </Form>
                <div className="mt-5">
                    <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation.</p>
                    <p><strong>Note:</strong> all re-identification requests are subject to breach monitoring and reporting</p>
                    <p>Details of breach thresholds can be found <Button size="sm" variant="link" onClick={()=> setShowBreachModal(true)}>here</Button>.</p>
                </div>
            </Card.Body>
        </Card>
    </Container>
    <BreachModal show={showBreachModal} hide={() => setShowBreachModal(false)} />
    </>
}

export default ReportsReasonPage