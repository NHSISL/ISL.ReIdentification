import React, { FunctionComponent } from "react";
import { Link } from 'react-router-dom';
import { UserAccess } from "../../models/userAccess/userAccess";
import { Button } from "react-bootstrap";
import moment from "moment";

type UserAccessRowProps = {
    userAccess: UserAccess;
}

const UserAccessRow: FunctionComponent<UserAccessRowProps> = (props) => {
    const {
        userAccess
    } = props;


    return (
        <tr>
            <td>{userAccess.firstName}</td>
            <td>{userAccess.lastName}</td>
            <td>
                <div className="p-2 rounded al text-center">
                    {userAccess.userEmail}
                </div>
            </td>
            <td>{userAccess.orgCode}</td>
            <td>{moment(userAccess.activeFrom?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            <td>{moment(userAccess.activeTo?.toString()).format("Do-MMM-yyyy HH:mm")}</td>
            <td>
                <Link to={`/userAccessDetail/${userAccess.id}`}>
                    <Button onClick={() => { }}>
                        Details
                    </Button>
                </Link>
            </td>

        </tr>
    );
}

export default UserAccessRow;