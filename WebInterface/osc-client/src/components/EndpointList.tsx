import { useState, useContext, useEffect } from "react";
import { EndpointConnectionCard, EndpointConnection, useStore } from "./EndpointConnection"
import { MIDIControlChangeMessage, MIDINoteMessage } from "../MIDIMessages";
import { NoteOnContext, NoteOffContext, ControlChangeContext } from "../context/MIDIContext";
import { Button } from "@blueprintjs/core";
import { generateId } from "../utils/generateId";
import { OSCContext } from '../context/OSCContext';
import { tryApplyProcessor } from "../utils/midiInput";
import { DEFAULT_ENDPOINT_NAME, DEFAULT_ENDPOINT_TYPE, DEFAULT_ENDPOINT_DEST } from "../definitions";
import OSC from 'osc-js';

export interface MessageProcessor {
    processMessage(message: any): void;
}

const ID_LENGTH = 10;

export function EndpointList() {
    const controlChange = useContext<MIDIControlChangeMessage | null>(ControlChangeContext);
    
    const osc = useContext(OSCContext);

    const endpoints = useStore((state : any) => state.endpoints);
    const updateEndpoint = useStore((state : any) => state.updateEndpoint);
    const addEndpoint = useStore((state : any) => state.addEndpoint);

    
    const loadEndpoints = () => {
        const endpoints = JSON.parse(localStorage.getItem("endpoints") || "[]");
        endpoints.forEach((endpoint : EndpointConnection) => {
            addEndpoint(endpoint);
        })
    };

    // useEffect(() => {
    //     loadEndpoints();
    // }, []);

    useEffect(() => {
        if (controlChange === null) {
            return;
        }
        // find an endpoint connection that has the id, then update its state
        const endpointsToUpdate = endpoints.filter((endpoint : EndpointConnection) => endpoint.midiId === controlChange.id);
        endpointsToUpdate.forEach((endpoint : EndpointConnection) => {
            const value = tryApplyProcessor(endpoint, controlChange.value);
            updateEndpoint(endpoint.id, {message : controlChange, currentValue : value});
            if (osc && endpoint.destination) {
                osc.send(new OSC.Message(endpoint.destination, value));      
            }
        })

        // find any that are listening
        const endpointsListening = endpoints.filter((endpoint : EndpointConnection) => endpoint.isListening);
        endpointsListening.forEach((endpoint : EndpointConnection) => {
            updateEndpoint(endpoint.id, {message : controlChange, midiId : controlChange.id, isListening : false})
        });

    }, [controlChange]);

    return (
        <>
            {endpoints.map((endpoint : EndpointConnection) => 
                <EndpointConnectionCard endpoint={endpoint} key={endpoint.id} />
            )}

            <Button onClick={() => {
                addEndpoint({
                    name: DEFAULT_ENDPOINT_NAME,
                    type: DEFAULT_ENDPOINT_TYPE,
                    id: generateId(ID_LENGTH),
                    destination: DEFAULT_ENDPOINT_DEST
                });
            }}>Add Endpoint</Button>


            <Button onClick={() => {
                localStorage.setItem("endpoints", JSON.stringify(endpoints));
            }}>Save Endpoints</Button>

            <Button onClick={() => {
                loadEndpoints();
            }}>Load Endpoints</Button>

        </>
    )
}