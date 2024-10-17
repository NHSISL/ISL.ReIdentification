import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import OdsTable from "../components/odsData/odsTable"
import OdsTree from "../components/odsData/odsTree"

export const OdsData = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="ODS Data">
                </BreadCrumbBase>
                <div className="mt-3">
                    <p>Role Needs to be in Security Matrix and Azure AD against user.</p>
                    <OdsTable />
                </div>
            </section>

            <OdsTree parentId="515A1642-65E6-4127-BC96-FA906987FB34"/>
        </Container>
    )
}