import { FunctionComponent  } from "react";

interface ImpersonationContextDetailManageProps {
    impersonationIdentificationRequestId: string | undefined;
}

const ImpersonationContextDetailManage: FunctionComponent<ImpersonationContextDetailManageProps> = ({ impersonationIdentificationRequestId }) => {
    return (
        <>
            {impersonationIdentificationRequestId}
        </>
    )
};

export default ImpersonationContextDetailManage;