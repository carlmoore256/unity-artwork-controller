import React, { createContext, useState, useEffect, useCallback } from 'react';
import { MIDINoteMessage, MIDIControlChangeMessage, MIDIMessage } from '../MIDIMessages';

export const NoteOnContext = createContext<MIDINoteMessage | null>(null);
export const NoteOffContext = createContext<MIDINoteMessage | null>(null);
export const ControlChangeContext = createContext<MIDIControlChangeMessage | null>(null);

function listInputsAndOutputs(midiAccess : MIDIAccess) {
    for (const entry of midiAccess.inputs) {
      const input = entry[1];
      console.log(
        `Input port [type:'${input.type}']` +
          ` id:'${input.id}'` +
          ` manufacturer:'${input.manufacturer}'` +
          ` name:'${input.name}'` +
          ` version:'${input.version}'`
      );
    }
  
    for (const entry of midiAccess.outputs) {
      const output = entry[1];
      console.log(
        `Output port [type:'${output.type}'] id:'${output.id}' manufacturer:'${output.manufacturer}' name:'${output.name}' version:'${output.version}'`
      );
    }
  }

function getMessageId(messageType : number, channel : number, parameter? : number) {
    return `${messageType}-${channel}-${parameter}`;
}

// Create a provider for the MIDI context
export const MIDIProvider = (props : { children : any }) => {
    const { children } = props;

    const [noteOn, setNoteOn] = useState<MIDINoteMessage | null>(null);
    const [noteOff, setNoteOff] = useState<MIDINoteMessage | null>(null);
    const [controlChange, setControlChange] = useState<MIDIControlChangeMessage | null>(null);

    const [midiAccess, setMidiAccess] = useState<any>(null);

    useEffect(() => {
        if (!navigator.requestMIDIAccess) {
            console.log("This browser does not support WebMIDI!");
            return; 
        }
        
        // request MIDI access
        console.log("Requesting MIDI access...");
        navigator.requestMIDIAccess().then(onMIDISuccess, onMIDIFailure);
    }, []);

    const onMIDISuccess = useCallback((_midiAccess : MIDIAccess) => {
        setMidiAccess(_midiAccess);
    }, [setMidiAccess])


    useEffect(() => {
        if (midiAccess === null) {
            return;
        }
        listInputsAndOutputs(midiAccess);
        midiAccess.inputs.forEach((input : MIDIInput) => {
            input.onmidimessage = handleMIDI;
        });
    }, [midiAccess]);

    function onMIDIFailure() {
        console.log('Could not access your MIDI devices.');
    }

    function handleMIDI(message : any) {
        // setMidiData(Array.from(message.data));

        const [status, ...data] = message.data;
        const messageType = status >> 4; // shift the upper 4 bits to the right
        const channel = status & 0xf; // bitwise AND with 1111 to get the lower 4 bits
        const id = getMessageId(messageType, channel, data[0]);
    
        switch (messageType) {
            case 8:
                setNoteOff({id, channel, parameter: data[0], value: data[1]});
                break;
            case 9:
                setNoteOn({id, channel, parameter: data[0], value: data[1]});
                break;
            case 11:
                setControlChange({id, channel, parameter: data[0], value: data[1]});
                break;
            // ... handle other message types
            default:
                console.log(`Unknown MIDI message type: ${messageType}`);
        }
    }

    // function handleMIDI(message : any) {
    //     console.log('MIDI message received', message);
    //     setMidiData(Array.from(message.data));
    // }

    return (
        <NoteOnContext.Provider value={noteOn}>
            <NoteOffContext.Provider value={noteOff}>
                <ControlChangeContext.Provider value={controlChange}>
                    {children}
                </ControlChangeContext.Provider>
            </NoteOffContext.Provider>
        </NoteOnContext.Provider>
    );
};
