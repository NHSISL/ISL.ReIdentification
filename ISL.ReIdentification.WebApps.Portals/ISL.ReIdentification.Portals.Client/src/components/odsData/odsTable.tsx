import React, { FunctionComponent } from "react";
import { Card, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUserDoctor } from "@fortawesome/free-solid-svg-icons";
import OdsRow from "./odsRow";

type OdsTableProps = {};

const OdsTable: FunctionComponent<OdsTableProps> = (props) => {
    return (
        <div className="infiniteScrollContainer">
            <Card>
                <Card.Header> <FontAwesomeIcon icon={faUserDoctor} className="me-2" /> ODS Data</Card.Header>
                <Card.Body>
                    <Table bordered>
                        <tbody>
                            <OdsRow></OdsRow>
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>

        </div>
    );
};

export default OdsTable;