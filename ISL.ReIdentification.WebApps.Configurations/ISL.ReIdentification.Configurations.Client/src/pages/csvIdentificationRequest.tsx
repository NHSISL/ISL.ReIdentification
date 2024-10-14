import { Container } from "react-bootstrap"
import BreadCrumbBase from "../components/bases/layouts/BreadCrumb/BreadCrumbBase"
import CsvIdentificationRequestTable from "../components/csvIdentificationRequest/csvIdentificationRequestTable"

export const CsvIdentificationRequestPage = () => {
    return (
        <Container fluid className="mt-4">
            <section>
                <BreadCrumbBase
                    link="/home"
                    backLink="Home"
                    currentLink="CSV Identification Request">
                </BreadCrumbBase>
                <div className="mt-3">
                    <p>Role Needs to be in Security Matrix and Azure AD against user.</p>
                    <CsvIdentificationRequestTable />
                </div>
            </section>
        </Container>
    )
}