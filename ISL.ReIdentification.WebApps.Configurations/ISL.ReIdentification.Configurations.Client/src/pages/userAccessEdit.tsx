import { Button, Card, CardBody, CardFooter, CardHeader, Container } from "react-bootstrap"
import { Link, useParams } from "react-router-dom"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import OdsTree from "../components/odsData/odsTree";
import { useEffect, useState } from "react";
import { OdsData } from "../models/odsData/odsData";
import { userAccessViewService } from "../services/views/userAccess/userAccessViewService";
import { UserAccess } from "../models/userAccess/userAccess";
import { odsDataService } from "../services/foundations/odsDataAccessService";

export const UserAccessEdit = () => {
    const { entraUserId } = useParams();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const { data } = userAccessViewService.useGetAccessForUser(entraUserId);
    const [selectedUser, setSelectedUser] = useState<UserAccess>();
    const [odsSearchString, setOdsSearchString] = useState("");
    const { data: rootRecord } = odsDataService.useRetrieveAllOdsData(odsSearchString);

    useEffect(() => {
        if (data && data?.length) {
            setSelectedUser(data[0]);
            const orgcodes = data.map(d => d.orgCode);
            setOdsSearchString(`?filter=OrganisationCode in (${orgcodes.map(x => `'${x}'`).join(',')})`)
        }
    }, [data])

    useEffect(() => {
        if (rootRecord) {
            console.log(rootRecord);
            setSelectedOdsRecords(rootRecord)
        }
    }, [rootRecord])

    const saveRecord = () => { }

    return (<Container fluid className="mt-4">
        <section>
            <BreadCrumbBase
                link="/userAccess"
                backLink="User Access"
                currentLink="Edit User">
            </BreadCrumbBase>
            <div className="mt-3">
                <Card>
                    <CardHeader>
                        Edit Account For:
                    </CardHeader>
                    <CardBody>
                        {selectedUser && <>
                            <div>display Name: {selectedUser.displayName}</div>
                            <div>Job Title: {selectedUser.jobTitle}</div>
                            <div>Mail: {selectedUser.email}</div>
                            <div>UPN: {selectedUser.userPrincipalName}</div>
                            <div style={{ paddingTop: "10px" }}>
                                <Card>
                                    <CardHeader>
                                        Select Organsisations {selectedUser.displayName} has access to:
                                    </CardHeader>
                                    <CardBody>
                                        <OdsTree rootName="Root" selectedRecords={selectedOdsRecords} setSelectedRecords={setSelectedOdsRecords} />
                                    </CardBody>
                                </Card>
                            </div>
                        </>
                        }
                    </CardBody>

                    <CardFooter>
                        <Button onClick={saveRecord}>Save</Button>
                        <Link to="/userAccess" style={{paddingLeft:'10px'}}>
                            <Button variant="secondary">Cancel</Button>
                        </Link>
                    </CardFooter>
                </Card>
            </div>
        </section>
    </Container>
    )
}