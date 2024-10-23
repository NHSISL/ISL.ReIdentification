import { FunctionComponent, useState } from "react";
import EntraUserSearch from "../EntraUserSearch/entraUserSearch";
import { entraUser } from "../../models/views/components/entraUsers/entraUsers";
import { Button, Card } from "react-bootstrap";

type UserAccessDetailCardNewProps = {
};

const UserAccessDetailCardNew: FunctionComponent<UserAccessDetailCardNewProps> = () => {
    const [selectedUser, setSelectedUser] = useState<entraUser | undefined>()


    return (
        <>
            {!selectedUser ? <EntraUserSearch selectUser={setSelectedUser}/> : <><Card>
                <Card.Body>
                    <div>display Name:&nbsp;{selectedUser.displayName}</div>
                    <div>email:&nbsp;{selectedUser.mail}</div>
                    <div>job title:&nbsp;{selectedUser.jobTitle}</div>
                    <div>upn:&nbsp;{selectedUser.userPrincipalName}</div>
                </Card.Body>    
                <Card.Footer>
                    <Button onClick={() => setSelectedUser(undefined)}>Back</Button>
                </Card.Footer>
            </Card>
            <hr/>
            <Card>
                <Card.Header>Select Orgs</Card.Header>
                <Card.Body>
                    
                </Card.Body>
            </Card>            
            
            </>}
        </>
    );
};

export default UserAccessDetailCardNew;