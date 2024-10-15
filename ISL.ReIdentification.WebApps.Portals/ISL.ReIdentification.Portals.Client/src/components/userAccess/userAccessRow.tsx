import React, { FunctionComponent } from "react";
import { UserAccess } from "../../models/userAccess/userAccess";
import UserAccessRowView from "./userAccessRowView";

type UserAccessRowProps = {
    userAccess: UserAccess;
};

const UserAccessRow: FunctionComponent<UserAccessRowProps> = (props) => {
    const {
        userAccess
    } = props;

    return (
        <UserAccessRowView
            key={userAccess.id.toString()}
            userAccess={userAccess}
        />
    );
};

export default UserAccessRow;