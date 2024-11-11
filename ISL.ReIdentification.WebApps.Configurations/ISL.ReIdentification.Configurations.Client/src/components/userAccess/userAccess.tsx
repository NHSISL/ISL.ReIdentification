import { Button, Container } from "react-bootstrap"
import BreadCrumbBase from "../bases/layouts/BreadCrumb/BreadCrumbBase"
import UserAccessTable from "./userAccessTable"
import { Link } from "react-router-dom"
import securityPoints from "../../securityMatrix"
import { SecuredComponent } from "../securitys/securedComponents"

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
                <SecuredComponent allowedRoles={securityPoints.userAccess.add}>
                    <Link to="/userAccess/newUser">
                        <Button>Add New User</Button>
                    </Link>
                </SecuredComponent>
            </section>
        </Container>
    )
}