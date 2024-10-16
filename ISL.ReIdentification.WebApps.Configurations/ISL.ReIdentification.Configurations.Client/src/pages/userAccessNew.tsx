import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import EntraUserSearch from "../components/EntraUserSearch/entraUserSearch"
import { useState } from "react"
import { entraUser } from "../models/views/components/entraUsers/entraUsers"
import OdsTree from "../components/odsData/odsTree"

export const UserAccessNew = () => {

    const [selectedUser, setSelectedUser] = useState<entraUser | undefined>();

    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="User Access">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h1>New User Access</h1>
                    {!selectedUser ? 
                        <EntraUserSearch selectUser={(entraUser) => { setSelectedUser(entraUser)}} />
                    : 
                        <>
                            <div>display Name: {selectedUser.displayName}</div>
                            <div>Job Title: {selectedUser.jobTitle}</div>
                            <div>Mail: {selectedUser.mail}</div>
                            <div>UPN: {selectedUser.userPrincipalName}</div>
                        </>
                    }

                    {selectedUser && <OdsTree parentId="515A1642-65E6-4127-BC96-FA906987FB34"/>}
                </div>
            </section>
        </Container>
    )
}