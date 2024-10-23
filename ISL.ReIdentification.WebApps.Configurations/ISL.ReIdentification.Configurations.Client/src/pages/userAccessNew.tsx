import { Button, ButtonGroup, Card, CardBody, CardFooter, CardHeader, Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import EntraUserSearch from "../components/EntraUserSearch/entraUserSearch"
import { useState } from "react"
import { entraUser } from "../models/views/components/entraUsers/entraUsers"
import OdsTree from "../components/odsData/odsTree"
import { OdsData } from "../models/odsData/odsData"
import { userAccessService } from "../services/foundations/userAccessService"
import { UserAccess } from "../models/userAccess/userAccess"

export const UserAccessNew = () => {

    const [selectedUser, setSelectedUser] = useState<entraUser | undefined>();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const {mutate} = userAccessService.useCreateUserAccess();

    const saveRecord = () => {
        const ua = new UserAccess({});

        if(selectedUser){
            ua.displayName = selectedUser.displayName; 
            ua.userEmail = selectedUser.mail;
            ua.jobTitle = selectedUser.jobTitle;
            ua.entraGuid = selectedUser.id;
            ua.entraUpn = selectedUser.userPrincipalName;
            ua.orgCodes = selectedOdsRecords.map(o => o.organisationCode);
        }

        mutate(ua);
    }

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/userAccess"
                    backLink="User Access"
                    currentLink="New User">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h1>New User Access</h1>
                    {!selectedUser ? 
                        <EntraUserSearch selectUser={(entraUser) => { setSelectedUser(entraUser)}} />
                    : 
                        <Card>
                            <CardHeader>
                                Create Account For:
                            </CardHeader>
                            <CardBody>
                                <div>display Name: {selectedUser.displayName}</div>
                                <div>Job Title: {selectedUser.jobTitle}</div>
                                <div>Mail: {selectedUser.mail}</div>
                                <div>UPN: {selectedUser.userPrincipalName}</div>
                                <div style={{paddingTop:"10px"}}>
                                <Card>
                                    <CardHeader>
                                        Select Organsisations {selectedUser.displayName} has access to:
                                    </CardHeader>
                                    <CardBody>
                                        <OdsTree rootName="Root" selectedRecords={selectedOdsRecords} setSelectedRecords={setSelectedOdsRecords}/>
                                    </CardBody>
                                </Card>
                                </div>
                            </CardBody>
                            
                            <CardFooter>
                                <ButtonGroup>
                                    <Button onClick={saveRecord}>Save</Button>
                                    <Button onClick={() => {
                                        setSelectedUser(undefined)
                                        setSelectedOdsRecords([]);
                                    } } variant="secondary">Clear</Button>
                                </ButtonGroup>
                            </CardFooter>
                        </Card>
                    }
                </div>
            </section>
        </Container>
    )
}