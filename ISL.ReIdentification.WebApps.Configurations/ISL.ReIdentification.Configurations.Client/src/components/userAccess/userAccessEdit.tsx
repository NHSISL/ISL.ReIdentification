import { Button, Card, CardBody, CardFooter, CardHeader, Container, Spinner } from "react-bootstrap"
import { Link, useNavigate, useParams } from "react-router-dom"
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase"
import OdsTree from "../odsData/odsTree";
import { useEffect, useState } from "react";
import { OdsData } from "../../models/odsData/odsData";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import { UserAccess } from "../../models/userAccess/userAccess";
import { odsDataService } from "../../services/foundations/odsDataAccessService";
import { userAccessService } from "../../services/foundations/userAccessService";
import { toastError } from "../../brokers/toastBroker.error";

export const UserAccessEdit = () => {
    const { entraUserId } = useParams();
    const [selectedOdsRecords, setSelectedOdsRecords] = useState<OdsData[]>([]);
    const { data, isLoading: isUserAccessLoading } = userAccessViewService.useGetAccessForUser(entraUserId);
    const [selectedUser, setSelectedUser] = useState<UserAccess>();
    const [odsSearchString, setOdsSearchString] = useState("");
    const { data: rootRecord, isLoading: isOdsDataLoading } = odsDataService.useRetrieveAllOdsData(odsSearchString);
    const { mutateAsync: createUserAccess } = userAccessService.useCreateUserAccess();
    const { mutateAsync: deleteUserAccess } = userAccessService.useRemoveUserAccess();
    const navigate = useNavigate();

    useEffect(() => {
        if (data && data?.length) {
            setSelectedUser(data[0]);
            const orgcodes = data.map(d => d.orgCode);
            setOdsSearchString(`?filter=OrganisationCode in (${orgcodes.map(x => `'${x}'`).join(',')})`)
        }
    }, [data])

    useEffect(() => {
        if (rootRecord) {
            setSelectedOdsRecords(rootRecord)
        }
    }, [rootRecord])

    const saveRecord = async () => {
        if (selectedUser) {
            const ua = {
                ...new UserAccess(),
                ...selectedUser,
                entraUserId: selectedUser.entraUserId,
                email: selectedUser.email,
                activeFrom: new Date(),
                orgCodes: selectedOdsRecords.map(x => x.organisationCode)
            }
            try {
                await createUserAccess(ua);
                navigate("/userAccess");
            } catch (error) {
                if (error instanceof Error) {
                    toastError(error.message);
                }
            }
        }
    }

    const deleteUser = async () => {
        if (data) {
            await data.forEach(async (ua) => {
                await deleteUserAccess(ua.id);
            });

            navigate("/userAccess");
        }
    }

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/userAccess"
                    backLink="User Access"
                    currentLink="Edit User">
                </BreadCrumbBase>
                <div className="mt-3">
                    {isUserAccessLoading || isOdsDataLoading ? (
                        <Spinner animation="border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </Spinner>
                    ) : (
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
                                {selectedOdsRecords.length === 0 ?
                                    <Button onClick={deleteUser}>Remove User</Button>
                                    :
                                    <Button onClick={saveRecord}>Save</Button>
                                }

                                <Link to="/userAccess" style={{ paddingLeft: '10px' }}>
                                    <Button variant="secondary">Cancel</Button>
                                </Link>
                            </CardFooter>
                        </Card>
                    )}
                </div>
            </section>
        </Container>
    )
}