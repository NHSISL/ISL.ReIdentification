import { FunctionComponent } from "react";
import { UserAccessView } from "../../models/views/components/userAccess/userAccessView";

interface UserAccessDetailCardViewProps {
    userAccess: UserAccessView;
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
