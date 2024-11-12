import { Button, ButtonGroup, Card, CardBody, CardFooter, CardHeader, Container, Spinner } from "react-bootstrap"
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase"
import EntraUserSearch from "../EntraUserSearch/entraUserSearch"
import { useState } from "react"
import { entraUser } from "../../models/views/components/entraUsers/entraUsers"
import OdsTree from "../odsData/odsTree"
import { OdsData } from "../../models/odsData/odsData"
import { userAccessService } from "../../services/foundations/userAccessService"
import { UserAccess } from "../../models/userAccess/userAccess"
import { useNavigate } from "react-router-dom"
import { toastError } from "../../brokers/toastBroker.error"

export const UserAccessNew = () => {

    const [selectedUser, setSelectedUser] = useState<entraUser | undefined>();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const navigate = useNavigate();
    const {mutateAsync, isPending, error} = userAccessService.useCreateUserAccess();

    const saveRecord = async () => {
        let ua : UserAccess;

        if(selectedUser){
            ua = {...new UserAccess(),                 
                ...selectedUser,
                entraUserId: selectedUser.id,
                email: selectedUser.mail,
                activeFrom: new Date(),
                orgCodes: selectedOdsRecords.map(x => x.organisationCode)
            }
            try {
                await mutateAsync(ua);
                navigate("/userAccess");
            } catch(error) {
                if(error instanceof Error) {
                    toastError(error.message);
                    console.log(error);
                }
            }
            
        } 
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
                                    <Button onClick={saveRecord} disabled={isPending}>
                                        { isPending ? <Spinner /> : "Save"}
                                    </Button>
                                    <Button onClick={() => {
                                        setSelectedUser(undefined)
                                        setSelectedOdsRecords([]);
                                    } } variant="secondary">Clear</Button>
                                </ButtonGroup>
                                {error && <div className="bs-warning">{JSON.stringify(error.message)}</div>}
                            </CardFooter>
                        </Card>
                    }
                </div>
            </section>
        </Container>
    )
}