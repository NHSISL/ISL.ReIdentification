import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import UserAccessTable from "../components/userAccess/userAccessTable"

export const UserAccess = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="User Access">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h1>User Access</h1>
                    <p>Role Needs to be in Security Matrix and Azure AD against user.</p>
                    <UserAccessTable />
                </div>
            </section>
        </Container>
    )
}