import React, { FunctionComponent, useState, useEffect } from "react";
import { ImpersonationContextView } from "../../models/views/components/impersonationContext/impersonationContextView";
import { impersonationContextViewService } from "../../services/views/impersonationContext/impersonationContextViewService";
import ImpersonationContextDetailCard from "./impersonationContextDetailCard";
import { useParams } from "react-router-dom";

type ImpersonationContextDetailProps = {
    children?: React.ReactNode;
};

const ImpersonationContextDetail: FunctionComponent<ImpersonationContextDetailProps> = (props) => {
    const {
        children
    } = props;

    const { ImpersonationContextId } = useParams();

    let impersonationContextRetrieved: ImpersonationContextView | undefined

    if (ImpersonationContextId !== "") {
        let { mappedImpersonationContext } = impersonationContextViewService.useGetImpersonationContextById(ImpersonationContextId);
        impersonationContextRetrieved = mappedImpersonationContext
    }

    const [impersonationContext, setImpersonationContext] = useState<ImpersonationContextView>();
    const [mode, setMode] = useState<string>('VIEW');

    useEffect(() => {
        if (ImpersonationContextId !== "") {
            setImpersonationContext(impersonationContextRetrieved);
            setMode('VIEW');
        }
        if (ImpersonationContextId === "" || ImpersonationContextId === undefined) {
            setImpersonationContext(new ImpersonationContextView(crypto.randomUUID(), "", "", "", "", new Date(), new Date()))
            setMode('ADD');
        }
    }, [ImpersonationContextId, impersonationContextRetrieved]);

    return (
        <div>
            {impersonationContext !== undefined && (
                <div>
                    <ImpersonationContextDetailCard
                        key={ImpersonationContextId}
                        impersonationContext={impersonationContext}
                        mode={mode}
                        //onAdd={handleAdd}
                        //onUpdate={handleUpdate}
                        //onDelete={handleDelete}
                    >
                        {children}
                    </ImpersonationContextDetailCard>
                </div>
            )}
        </div>
    );
};

export default ImpersonationContextDetail;