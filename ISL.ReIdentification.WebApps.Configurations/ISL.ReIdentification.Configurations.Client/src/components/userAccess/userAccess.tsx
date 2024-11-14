import { Alert, Button, Container } from "react-bootstrap"
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
                    <p>Use this screen to assign reidentification users to ODS organisations, this will allow an end user to reidentify patients that are registered to the ODS code and patients registered to all children ODS organisations.</p>
                    <Alert variant="danger">Any modification to data on this screen is audited.</Alert>
                    <UserAccessTable />
                </div>
               
            </section>
        </Container>
    )
}