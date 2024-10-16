import { Container } from "react-bootstrap";
import { useParams } from "react-router-dom";

export const Page1 = () => {
    const { id } = useParams();

    return (
        <Container fluid>
            <h1>Page 1</h1>
            <p>This is an example how to get the url param into the page - {id}</p>
        </Container>
    )
}