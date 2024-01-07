import { useState, useEffect, useCallback, useContext } from 'react'
import {
    NoteOnContext,
    NoteOffContext,
    ControlChangeContext
} from '../context/MIDIContext';
import {
    MIDINoteMessage,
    MIDIControlChangeMessage,
    MIDINoteToString
} from '../MIDIMessages';
import { Button, Card, Elevation, Pre, H5, Collapse } from "@blueprintjs/core";

interface MIDINoteLogMessage {
    note: MIDINoteMessage;
    time: Date;
    type: "noteOn" | "noteOff";
}


function MIDINoteLogMessageToString(message: MIDINoteLogMessage) {
    const noteTypeMessage = message.type === "noteOn" ? "ON " : "OFF";
    return `[${message.time.toISOString().slice(11, 19)}] ${noteTypeMessage} ${MIDINoteToString(message.note)}`;
}


const LOG_LENGTH = 8;

export function MIDINoteMonitor(props: {}) {
    const noteOn = useContext(NoteOnContext);
    const noteOff = useContext(NoteOffContext);

    const [noteLog, setNoteLog] = useState<MIDINoteLogMessage[]>([]);
    const [noteLogString, setNoteLogString] = useState<string>("");
    const [isOpen, setIsOpen] = useState<boolean>(false);

    useEffect(() => {
        if (noteOn !== null) {
            // console.log("YO got some stuff in the component " + MIDINoteToString(noteOn));
            setNoteLog([...noteLog, { note: noteOn, time: new Date(), type: "noteOn"}]);
        }
    }, [noteOn]);


    useEffect(() => {
        if (noteOff !== null) {
            setNoteLog([...noteLog, { note: noteOff, time: new Date(), type: "noteOff" }]);
        }
    }, [noteOff])

    useEffect(() => {
        if (noteLog.length > LOG_LENGTH) {
            setNoteLog(noteLog.slice(1));
        }

        setNoteLogString(noteLog.map((note) => {
            return MIDINoteLogMessageToString(note);
        }).join("\n"));
    }, [noteLog])


    // style={{backgroundColor: "#2F343C"}}
    return (
        <>
        <div className='item'>
            <Card interactive={true} onClick={() => setIsOpen(!isOpen)} elevation={Elevation.TWO}>
                <H5>
                    MIDI Note Monitor
                </H5>
                <Collapse isOpen={isOpen} keepChildrenMounted={false}>
                    <Pre>
                        {noteLogString}
                    </Pre>
                </Collapse>
            </Card>
        </div>
        </>
    )
}