import React, { FunctionComponent } from "react";
import { Card, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faAddressBook } from "@fortawesome/free-solid-svg-icons";
import PdsRow from "./pdsRow";

type PdsTableProps = {};

const PdsTable: FunctionComponent<PdsTableProps> = (props) => {
    return (
        <div className="infiniteScrollContainer">
            <Card>
                <Card.Header> <FontAwesomeIcon icon={faAddressBook} className="me-2" /> PDS Data</Card.Header>
                <Card.Body>
                    <Table bordered>
                        <tbody>
                            <PdsRow></PdsRow>
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>

        </div>
    );
};

export default PdsTable;