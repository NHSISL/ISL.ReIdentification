import { faCheck, faCopy } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { FunctionComponent, useState } from "react"
import { Button, Card, Form } from "react-bootstrap"

const ReportHelper: FunctionComponent = () => {
    const [reportGroupId, setReportGroupId] = useState("");
    const [reportId, setReportId] = useState("");
    const [psuedoColumn, setPsuedoColumn] = useState("");
    const [reportPage, setReportPage] = useState("");
    const [url, setUrl] = useState("");
    const [hasCopied, setHasCopied] = useState(false)


    function generateUrl(): void {
        let url = `${window.location.href}/${reportGroupId}/${reportId}/${psuedoColumn}`;
        if(reportPage){
            url += `/${reportPage}`;
        }
        setUrl(url);
    }

    function copyUrl(): void {
        navigator.clipboard.writeText(url);
        setHasCopied(true);
        setTimeout(() => { setHasCopied(false)}, 2000);
    }

    return (<>
        <Card>
            <Card.Header>Helper</Card.Header>
            <Card.Body>
                <p>Use this tool to help you construct the url for the reidentification server.</p>
                <Form>
                    <Form.Group>
                        <Form.Label>Report Group Id:</Form.Label>
                        <Form.Control value={reportGroupId} onChange={(e) => { setReportGroupId(e.target.value)}} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Report Id:</Form.Label>
                        <Form.Control value={reportId} onChange={(e) => { setReportId(e.target.value)}} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Column Containing Pseudo Identifer</Form.Label>
                        <Form.Control value={psuedoColumn} onChange={(e) => { setPsuedoColumn(e.target.value)}} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Report Page to launch (Leave blank to launch the home page of the report)</Form.Label>
                        <Form.Control value={reportPage} onChange={(e) => { setReportPage(e.target.value)}} />
                    </Form.Group>
                    <Button onClick={generateUrl}>Generate</Button>
                    <br/>
                    {url && <div>Url: {url} <FontAwesomeIcon icon={hasCopied ? faCheck : faCopy} onClick={copyUrl} /></div>}
                </Form>
            </Card.Body>
        </Card>
    </>)
}

export default ReportHelper;