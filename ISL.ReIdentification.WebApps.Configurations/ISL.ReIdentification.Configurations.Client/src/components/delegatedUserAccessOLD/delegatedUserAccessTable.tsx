import React, { FunctionComponent } from "react";
import { Card, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUserFriends } from "@fortawesome/free-solid-svg-icons";
import DelegatedUserAccessRow from "./delegatedUserAccessRowView";


type DelegatedUserAccessTableProps = {};

const DelegatedUserAccessTable: FunctionComponent<DelegatedUserAccessTableProps> = (props) => {
    return (
        <div className="infiniteScrollContainer">
            <Card>
                <Card.Header> <FontAwesomeIcon icon={faUserFriends} className="me-2" /> Delegated User Access</Card.Header>
                <Card.Body>
                    <Table bordered>
                        <tbody>
                            <DelegatedUserAccessRow></DelegatedUserAccessRow>
                        </tbody>
                    </Table>
                </Card.Body>
            </Card>

        </div>
    );
};

export default DelegatedUserAccessTable;