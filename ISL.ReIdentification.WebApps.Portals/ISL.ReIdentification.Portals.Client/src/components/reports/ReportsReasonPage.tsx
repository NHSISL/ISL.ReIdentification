import { FunctionComponent } from "react";
import { Button, Card, Container, Form, FormGroup, Spinner } from "react-bootstrap";
import { lookupViewService } from "../../services/views/lookups/lookupViewService";

type ReportReasonPageProps = {
    setReidReason: (reason: string) => void;
    launchReport: () => void;
    reidReason: string;
};

const ReportsReasonPage: FunctionComponent<ReportReasonPageProps> = (props: ReportReasonPageProps) => {
    const { setReidReason, launchReport, reidReason } = props
    const { mappedLookups, isLoading } = lookupViewService.useGetAllLookups("");

    return <><Container>
        <Card>
            <Card.Header>
                ISL Reidentification Portal
            </Card.Header>
            <Card.Body>
                <div>
                    <p>This page provides a simple reidentification for a single patient pseudo identifer</p>
                    <p><strong>Note:</strong> you will only be able to reidentify patients that are present within your organisation.</p>
                    <p><strong>Note:</strong> all reidentification requests are subject to breach monitoring and reporting</p>
                    <p>Details of breach thresholds can be found <a href="about:blank" target="blank" >here</a></p>
                </div>
                <Form>
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
            </Card.Body>
        </Card>
    </Container></>
}

export default ReportsReasonPage