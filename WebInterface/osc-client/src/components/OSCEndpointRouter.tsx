import { useState, useEffect } from 'react';
import { Card } from '@blueprintjs/core';

import { EndpointChain } from './EndpointSelector';
import { UNITY_ENDPOINTS } from '../data/defaultEndpoints';
import { IEndpoint } from '../data/unityEndpoints';
import { EndpointPath } from '../data/EndpointPath';
import { EndpointSelector } from './EndpointSelector';

// path={new EndpointPath()} 
export function OSCEndpointRouter(props : {}) {

    const [endpoints, setEndpoints] = useState<string | null>(null);
    // const [path, setPath] = useState<string[]>([]);
    const path : string[] = [];

    const onPathUpdate = (path: string[]) => {
        console.log("path updated", path);
    };

    return (
        <>
            <Card>
                <div>
                    <h2>OSC Endpoint Router</h2>
                </div>
                <div>
                    <h3>Endpoints</h3>
                    <p>{endpoints}</p>
                    {/* <EndpointChain endpoint={UNITY_ENDPOINTS} path={path} onPathUpdate={onPathUpdate}/> */}
                    <EndpointSelector endpoint={UNITY_ENDPOINTS} onPathUpdate={onPathUpdate}/>
                </div>
            </Card>
        </>
  )
}

export interface OSCEndpoint {
    address : string;

}


export function OSCEndpoint (props : {}) {

    return (
        <>
            <Card>
                <div>
                    <h2>OSCEndpoint</h2>
                </div>
            </Card>
        </>
    )
}