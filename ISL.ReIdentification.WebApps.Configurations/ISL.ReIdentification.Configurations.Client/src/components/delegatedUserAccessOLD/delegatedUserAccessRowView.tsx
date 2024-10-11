import React, { FunctionComponent } from "react";
import { Button } from "react-bootstrap";

type DelegatedUserAccessRowProps = {}

const DelegatedUserAccessRow: FunctionComponent<DelegatedUserAccessRowProps> = () => {

    return (
        <>
        <tr>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 1
                </span>
            </td>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 2
                </span>
            </td>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 3
                </span>
            </td>

            <td>
                <Button onClick={() => { }}>
                    Details
                </Button>
            </td>
        </tr>


         <tr>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 1
                </span>
            </td>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 2
                </span>
            </td>
            <td>
                <span className="p-2 rounded al text-center">
                    Col 3
                </span>
            </td>

            <td>
                <Button onClick={() => { }}>
                    Details
                </Button>
            </td>
            </tr>
        </>
    );
}

export default DelegatedUserAccessRow;