import React, { FunctionComponent } from "react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCirclePlus } from '@fortawesome/free-solid-svg-icons'
import { Button } from "react-bootstrap";
import { SecuredComponent } from "../../securitys/securedComponents";

interface LookupRowNewProps {
    onAdd: (value: boolean) => void;
}

const LookupRowNew: FunctionComponent<LookupRowNewProps> = (props) => {
    const {
        onAdd
    } = props;

    return (
        <SecuredComponent>
            <tr>
                <td></td>
                <td></td>
                <td className="text-end">
                    <Button id="supplierssAdd" onClick={() => onAdd(true)}>
                        <FontAwesomeIcon icon={faCirclePlus} size="lg" />&nbsp; New
                    </Button>
                </td>
            </tr>
        </SecuredComponent>
    );
}

export default LookupRowNew;