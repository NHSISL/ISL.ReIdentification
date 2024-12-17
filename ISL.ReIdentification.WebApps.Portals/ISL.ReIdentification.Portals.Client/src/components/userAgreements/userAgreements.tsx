import { Button, Modal } from "react-bootstrap"
import { AuthenticatedTemplate, useMsal } from "@azure/msal-react"
import { userAgreementService } from "../../services/foundations/userAgreementService";
import { UserAgreement } from "../../models/userAgreement/userAgreement";
import { SpinnerBase } from "../bases/spinner/SpinnerBase";
import { useAgreementCatalogue } from "./agreementCatalogue";
import { useFrontendConfiguration } from "../../hooks/useFrontendConfiguration";


export const UserAgreementModal = () => {
    const { accounts, instance } = useMsal();
    const currentAgreementType = "UserAgreement";
    const { configuration } = useFrontendConfiguration();
    const { data: hasAgreed, isLoading } = userAgreementService.useRetrieveAgreement(accounts[0]?.localAccountId, configuration?.activeAgreement, currentAgreementType);
    const { mutate, isPending, isSuccess } = userAgreementService.useAcceptAgreement();
    const { getAgreement } = useAgreementCatalogue();
    

    function agree(): void {
        const newAgreement = {
            id: crypto.randomUUID(),
            entraUserId: accounts[0].localAccountId,
            agreementVersion: configuration?.activeAgreement,
            agreementType: currentAgreementType,
            agreementDate: new Date(),
            createdDate: new Date(),
            createdBy: accounts[0].username,
            updatedDate: new Date(),
            updatedBy: accounts[0].username
        } as UserAgreement;

        mutate(newAgreement);
    }

    if(configuration?.activeAgreement === undefined) {
        return <Modal>Configuration does not define user agreement.</Modal>
    }

    return <AuthenticatedTemplate>
        {isLoading || isSuccess ? <></> :
            <Modal show={!hasAgreed} size="lg">
                <Modal.Header>
                    <Modal.Title>User Agreement - Version {configuration?.activeAgreement} </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>You must accept the following user agreement before you can use this application.</p>
                    {getAgreement(currentAgreementType, configuration.activeAgreement)}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => { instance.logoutRedirect() }}>Decline and Logout</Button>
                    <Button variant="primary" disabled={isPending} onClick={agree}>
                        {isPending ? <SpinnerBase /> : <>Accept</>}
                    </Button>
                </Modal.Footer>
            </Modal>
        }
    </AuthenticatedTemplate>
}