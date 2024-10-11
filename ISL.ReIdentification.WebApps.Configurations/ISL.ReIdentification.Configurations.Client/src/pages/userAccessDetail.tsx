import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"

export const UserAccessDetail = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/userAccess"
                    backLink="User Access"
                    currentLink="User Access Detail">
                </BreadCrumbBase>
                <div className="mt-3">
                    <h1>User Access Details</h1>
                    <UserAccessDetail />
                </div>
            </section>
        </Container>
    )
}