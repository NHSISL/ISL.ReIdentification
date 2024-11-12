import { Container, Row, Col } from "react-bootstrap";
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase";
import ReportHelper from "../components/reports/reportHelper";

export const ReIdReportHome = () => {
    return (
        <>
            <Container fluid className="mt-4">
                <section>
                    <BreadCrumbBase
                        link="/"
                        backLink="Home"
                        currentLink="Reports">
                    </BreadCrumbBase>

                    <Row className="mt-3">
                        <Col md={6} className="col1">
                            <ReportHelper />
                        </Col>
                        <Col md={6} className="col2">

                            <div className="bg-light p-3">
                                <h5>Report Url Generator Help</h5>
                                <p>The ISL reidentification portal provides 3 types of integration into to BI tools:</p>
                                <ol>
                                    <li>Seemless integration to reidentify one patient within PowerBI.</li>
                                    <li>Seemless integration to reidentify a group of patients within PowerBI.</li>
                                    <li>A popup web page that can be integrated into any web based BI tool that can reidentify a single patient.</li>
                                </ol>
                                <p>This page intended for Analysts and provides instructions and tools to enable this integration.</p>
                                <h5>PowerBI Single Patient Reidentification</h5>
                                <p>Power BI Reidentification setup up requires X steps to configure (Click on each step for further information):</p>
                                <ol>
                                    <li>Prepare the report dataset for re-identfication by including the Pseudo identifier.</li>
                                    <li>Publish the report to Power BI server and extract the report identifers.</li>
                                    <li>Generate unique link to reidentification portal for report.</li>
                                </ol>
                                <h5>PowerBI Multiple Patient Reidentification </h5>
                                <p>Power BI Reidentification setup up requires X steps to configure (Click on each step for further information):</p>
                                <ol>
                                    <li>Create a measure on PowerBi that includes a comma seperated list of Pseudo identifers.</li>
                                    <li>Publish the report to Power BI server and extract the report identifers.</li>
                                    <li>Generate unique link to reidentification portal for report.</li>
                                </ol>
                                <h5>Reidentification Popup</h5>
                                <p>The reidentification service provices a url that can be direct linked to from PowerBI or Tableau this will provide a single record re-identification, this can either be launched in a new tab or hosted within a webview or iframe within the BI tool:</p>
                                <ol>
                                    <li>Understand how to construct the re-identification URL</li>
                                    <li>PowerBI generate a weblink</li>
                                    <li>Tableau generate a weblink</li>
                                </ol>
                            </div>
                        </Col>
                    </Row>
                </section>
            </Container>
        </>
    )
}