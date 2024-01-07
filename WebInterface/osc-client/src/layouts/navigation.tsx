import { Navbar, Alignment, Button } from "@blueprintjs/core"

export function Navigation(props : {}) {
    

    return (
        <>
        <Navbar>
            <Navbar.Group align={Alignment.LEFT}>
                <Navbar.Heading>Blueprint</Navbar.Heading>
                <Navbar.Divider />
                <Button className="bp4-minimal" icon="home" text="Home" />
                <Button className="bp4-minimal" icon="document" text="Files" />
            </Navbar.Group>
        </Navbar>
        </>
    )
}