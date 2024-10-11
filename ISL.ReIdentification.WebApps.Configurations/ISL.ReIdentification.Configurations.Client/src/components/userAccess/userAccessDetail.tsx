import React, { FunctionComponent, useState, useEffect } from "react";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";
import { userAccessViewService } from "../../services/views/userAccess/userAccessViewService";
import UserAccessDetailCard from "./userAccessDetailCard";
import { useParams } from "react-router-dom";

type UserAccessDetailProps = {
    //userAccessId?: string;
    children?: React.ReactNode;
};

const UserAccessDetail: FunctionComponent<UserAccessDetailProps> = (props) => {
    const {
        //userAccessId,
        children
    } = props;

    const { userDetailId } = useParams();

    let userAccessRetrieved: UserAccessView | undefined

    if (userDetailId !== "") {
        let { mappedUserAccess } = userAccessViewService.useGetUserAccessById(userDetailId);
        userAccessRetrieved = mappedUserAccess
    }

    const [userAccess, setUserAccess] = useState<UserAccessView>();
    const [mode, setMode] = useState<string>('VIEW');

    useEffect(() => {
        if (userDetailId !== "") {
            setUserAccess(userAccessRetrieved);
            setMode('VIEW');
        }
        if (userDetailId === "" || userDetailId === undefined) {
            setUserAccess(new UserAccessView(crypto.randomUUID(), "", "", "", "", new Date(), new Date()))
            setMode('ADD');
        }
    }, [userDetailId, userAccessRetrieved]);

    return (
        <div>
            {userAccess !== undefined && (
                <div>
                    <UserAccessDetailCard
                        key={userDetailId}
                        userAccess={userAccess}
                        mode={mode}
                        //onAdd={handleAdd}
                        //onUpdate={handleUpdate}
                        //onDelete={handleDelete}
                    >
                        {children}
                    </UserAccessDetailCard>
                </div>
            )}
        </div>
    );
};

export default UserAccessDetail;