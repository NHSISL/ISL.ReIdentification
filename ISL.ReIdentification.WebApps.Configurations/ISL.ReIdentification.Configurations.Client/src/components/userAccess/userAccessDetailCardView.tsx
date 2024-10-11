import React, { FunctionComponent } from "react";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { Form } from "react-bootstrap";

interface UserAccessDetailCardViewProps {
    userAccess: UserAccessView;
    //onDelete: (userAccess: UserAccessView) => void;
    mode: string;
    onModeChange: (value: string) => void;
}

const UserAccessDetailCardView: FunctionComponent<UserAccessDetailCardViewProps> = (props) => {
    const {
        userAccess,
    } = props;


    return (
        <>
            <h1>Daves Form in ReadOnly To Replace</h1>
            {userAccess.id}
            
        </>
    );
}

export default UserAccessDetailCardView;
