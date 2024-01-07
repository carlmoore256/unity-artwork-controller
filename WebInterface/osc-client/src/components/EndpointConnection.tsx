import { useState, useEffect, useCallback, useContext } from 'react';
import { Card, Button, InputGroup, ButtonGroup, EditableText, TextArea, H5, Label } from "@blueprintjs/core";
import { create } from 'zustand';
import { MIDIControlChangeMessage, MIDINoteMessage } from '../MIDIMessages';
import { CodeEditorDialog } from './CodeEditorDialog';
import { DEFAULT_PROCESSOR } from '../definitions';
import { EndpointSelector } from './EndpointSelector';
import { UNITY_ENDPOINTS } from '../data/defaultEndpoints';

export const useStore = create(set => ({
    endpoints: [],
    updateEndpoint: (id: string, updatedEndpoint: Partial<EndpointConnection>) => {
        console.log("UPDATING ENDPOINT " + id + " " + JSON.stringify(updatedEndpoint));
        set((state : any) => ({
            endpoints: state.endpoints.map((endpoint : any) =>
            endpoint.id === id ? {...endpoint, ...updatedEndpoint} : endpoint
            ),
        }))
    },
    addEndpoint: (endpoint: EndpointConnection) => {
        if (!endpoint.processor) {
            endpoint.processor = DEFAULT_PROCESSOR;
        }
        set((state : any) => (
            { endpoints: [...state.endpoints, endpoint] }
        ))
    },
    deleteEndpoint: (idToDelete : string) => {
        set((state : any) => (
            { endpoints: state.endpoints.filter((endpoint : EndpointConnection) => endpoint.id !== idToDelete) }
        ))
    },
  }));
  

export interface EndpointConnection {
    id : string;
    midiId? : string;
    name : string;
    type : string;
    isListening : boolean;
    destination?: string; // the OSC endpoint to route the message to
    message? : MIDIControlChangeMessage | MIDINoteMessage;
    currentValue? : number;
    processor: string;
}
    

// represents an endpoint, which is a card that displays the midi input
// and the destination OSC endpoint

export function EndpointConnectionCard(props : { endpoint : EndpointConnection }) {

    const { endpoint } = props;

    const [destination, setDestination] = useState<string>(endpoint.destination || "");

    const isAssigned = endpoint.midiId !== undefined;
    var backgroundColor = isAssigned ? "#ffffff" : "#f78686";
    if (endpoint.isListening) {
        backgroundColor = "#b2d6eb";
    }

    const [isCodeOpen, setIsCodeOpen] = useState<boolean>(false);
    const updateEndpoint = useStore((state : any) => state.updateEndpoint);
    const deleteEndpoint = useStore((state : any) => state.deleteEndpoint);

    useEffect(() => {
        endpoint.destination = destination;
        updateEndpoint({ ...endpoint });
        setDestination(endpoint.destination || "");
    }, [destination]);


    // const handleDestinationChange = useCallback((e : any) => {
    //     endpoint.destination = e.target.value;
    //     setDestination(e.target.value);
    //     updateEndpoint({ ...endpoint });
    // }, [destination]);

    const handleDelete = () => {
        deleteEndpoint(endpoint.id);
    };

    const handleListen = () => {
        endpoint.isListening = !endpoint.isListening;
        updateEndpoint({ ...endpoint });
    };

    const handleNameChange = (name : string) => {
        endpoint.name = name;
        updateEndpoint({ ...endpoint });
    };

    const handleCodeDialogSubmit = (submittedCode: string) => {
        // setCode(submittedCode);
        endpoint.processor = submittedCode;
        updateEndpoint({ ...endpoint });
    };

    return (
        <Card style={{backgroundColor: backgroundColor}} className="endpoint-card">
            <H5>
                <EditableText defaultValue={`${endpoint.name}`} onConfirm={handleNameChange}/>
            </H5>
            <div className="endpoint-card-section">
                <p> Type: <em>{endpoint.type}</em></p>
            </div>

            <div className="endpoint-card-section">
                <p>Value: <strong>{endpoint.message && endpoint.message.value}</strong> {"->"} {endpoint.currentValue?.toPrecision(5)} </p>
            </div>

            <div className="endpoint-card-section">
                <p>MIDI ID : {endpoint.midiId ? <strong>{endpoint.midiId}</strong> : null}</p>
            </div>

            <div className="endpoint-card-destination">
                <Label>Destination</Label>
                <InputGroup
                    value={endpoint.destination}
                    onChange={e => setDestination(e.target.value)}
                    placeholder="Destination"
                />
            </div>

            <EndpointSelector endpoint={UNITY_ENDPOINTS} onPathUpdate={(path) => {
                console.log("Endpoint updated", path.join("/"));
                setDestination(path.join("/"));
            }}/>





            <div className="endpoint-card-buttons">
                <ButtonGroup minimal={false}>
                    <Button onClick={handleListen}>Listen</Button>
                    <Button onClick={() => setIsCodeOpen(!isCodeOpen)}>Edit Processor</Button>
                    <Button onClick={handleDelete}>Delete</Button>
                </ButtonGroup>

            </div>


            <CodeEditorDialog 
                title="Edit Processor Code (Javascript)"
                isOpen={isCodeOpen} 
                onClose={() => setIsCodeOpen(false)} 
                onSubmit={handleCodeDialogSubmit} 
            />

            

        </Card>
    );
}