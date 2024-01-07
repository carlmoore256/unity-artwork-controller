import { useState, useEffect, useCallback, useContext } from 'react'
import {
    NoteOnContext,
    NoteOffContext,
    ControlChangeContext
} from '../context/MIDIContext';
import {
    MIDIControlChangeMessage,
    MIDIControlToString
} from '../MIDIMessages';
import { Button, Card, Elevation, Pre, H5, Collapse } from "@blueprintjs/core";

interface MIDIControlLogMessage {
    control: MIDIControlChangeMessage;
    time: Date;
}


function MIDIControlLogMessageToString(message: MIDIControlLogMessage) {
    return `[${message.time.toISOString().slice(11, 19)}] ${MIDIControlToString(message.control)} | ${message.control.id}`;
}


const LOG_LENGTH = 8;

export function MIDIControlMonitor(props: {}) {
    const controlChange = useContext(ControlChangeContext);

    const [controlLog, setControlLog] = useState<MIDIControlLogMessage[]>([]);
    const [controlLogString, setControlLogString] = useState<string>("");
    const [isOpen, setIsOpen] = useState<boolean>(false);


    useEffect(() => {
        if (controlChange !== null) {
            setControlLog([...controlLog, { control: controlChange, time: new Date() }]);
        }
    }, [controlChange]);

    useEffect(() => {
        if (controlLog.length > LOG_LENGTH) {
            setControlLog(controlLog.slice(1));
        }

        setControlLogString(controlLog.map((note) => {
            return MIDIControlLogMessageToString(note);
        }).join("\n"));
    }, [controlLog])


    // style={{backgroundColor: "#2F343C"}}
    return (
        <>
        <div className='item'>
            <Card interactive={true} onClick={() => setIsOpen(!isOpen)} elevation={Elevation.TWO}>
                <H5>
                    MIDI Control Monitor
                </H5>
                <Collapse isOpen={isOpen} keepChildrenMounted={false}>
                    <Pre>
                        {controlLogString}
                    </Pre>
                </Collapse>
            </Card>
        </div>
        </>
    )
}