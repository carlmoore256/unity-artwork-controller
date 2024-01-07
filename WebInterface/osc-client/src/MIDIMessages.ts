export enum MIDIMessageType {
    NoteOn,
    NoteOff,
    ControlChange,
    ProgramChange,
    PitchBend,
}

export interface MIDIMessage {
    id : number;
    channel : number;
    parameter : number;
    value : number;
    type : MIDIMessageType;
}


export interface MIDINoteMessage {
    id : string;
    channel : number;
    parameter : number;
    value : number;
}

export interface MIDIControlChangeMessage {
    id : string;
    channel : number;
    parameter : number;
    value : number;
}

export function MIDINoteToString(note : MIDINoteMessage) : string {
    return `Pitch: ${note.parameter} | Velocity: ${note.value} | Channel: ${note.channel}`;
}

export function MIDIControlToString(control : MIDIControlChangeMessage) : string {
    return `Control: ${control.parameter} | Value: ${control.value} | Channel: ${control.channel}`;
}
