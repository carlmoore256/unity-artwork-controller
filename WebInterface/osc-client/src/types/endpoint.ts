import { MIDINoteMessage, MIDIControlChangeMessage } from '../MIDIMessages';

export type Message = MIDINoteMessage | MIDIControlChangeMessage;

export interface MessageBinding {
    message: MIDINoteMessage | MIDIControlChangeMessage;

}

// input: Message; // some sort of MIDI input;