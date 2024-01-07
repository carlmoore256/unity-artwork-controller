import { useState, useEffect, useContext, useRef } from 'react';
import { HTMLSelect } from '@blueprintjs/core';
import { IEndpoint } from '../data/unityEndpoints';
import { VariableDropdown } from './VariableDropdown';
import { Variable } from '../data/Variable';
import { GetVariable, AllVariables } from '../context/VariableContext';
import { EndpointPath } from '../data/EndpointPath';

interface EndpointDropdownProps {
    endpoint: IEndpoint;
    onEndpointSelect: (selectedEndpoint: IEndpoint | null) => void;
}

function EndpointDropdown(props: EndpointDropdownProps) {
    const { endpoint, onEndpointSelect } = props;
    return (
        <>
            <HTMLSelect
                onChange={(e) => onEndpointSelect(endpoint.children.find(child => child.id === e.target.value) || null)}
            >
                <option value="">Select...</option>
                {endpoint.children.map(child => (
                    <option key={child.id} value={child.id}>{child.label}</option>
                ))}
            </HTMLSelect>
        </>
    );
};


// interface EndpointChainProps {
//     endpoint: IEndpoint;
//     path: EndpointPath;
//     level?: number;
// }


interface EndpointChainProps {
    endpoint: IEndpoint;
    path: string[];
    onPathUpdate: (path: string[]) => void;
}

export function EndpointChain(props: EndpointChainProps) {
    const { endpoint, path, onPathUpdate } = props;
    const [selectedEndpoint, setSelectedEndpoint] = useState<IEndpoint | null>(null);
    const [selectedVariable, setSelectedVariable] = useState<Variable | null>(null);
    const [newPath, setNewPath] = useState<string[]>([]);

    useEffect(() => {
        let updatedPath = [...path];
        if (selectedVariable) {
            updatedPath.push(selectedVariable.value);

            
        } 

        if (selectedEndpoint) {
            updatedPath.push(selectedEndpoint.directory);
        }
        setNewPath(updatedPath);    
        onPathUpdate(updatedPath);
    
    }, [selectedVariable, selectedEndpoint]);


    if (endpoint.children.length > 0 && endpoint.children[0].type === "variable") {
        return (
            <>
                <VariableDropdown setSelectedVariable={setSelectedVariable} />
                <EndpointDropdown endpoint={endpoint.children[0]} onEndpointSelect={setSelectedEndpoint} />
                {selectedEndpoint && selectedEndpoint.children.length > 0 && (
                    <EndpointChain endpoint={selectedEndpoint} path={newPath} onPathUpdate={onPathUpdate} />
                )}
            </>
        );
    }

    return (
        <div style={{ display: "flex" }}>
            <EndpointDropdown endpoint={endpoint} onEndpointSelect={setSelectedEndpoint} />
            {selectedEndpoint && selectedEndpoint.children.length > 0 && (
                <EndpointChain endpoint={selectedEndpoint} path={newPath} onPathUpdate={onPathUpdate} />
            )}
        </div>
    );
}


interface EndpointSelectorProps {
    endpoint: IEndpoint;
    onPathUpdate: (path: string[]) => void;
}

export function EndpointSelector({ endpoint, onPathUpdate }: EndpointSelectorProps) {
    const pathRef = useRef<string[]>([]);

    useEffect(() => {
        pathRef.current = []; // Reset path ref at the start of each render
    }, []);

    // Callback function to be passed to each EndpointChain component
    const handlePathUpdate = (updatedPath: string[]) => {
        pathRef.current = updatedPath;
        onPathUpdate(pathRef.current); // Send the latest path up to the parent component
    }
    return (
        <div>
            <EndpointChain endpoint={endpoint} path={[]} onPathUpdate={handlePathUpdate} />
        </div>
    );
}





function getFullPath(endpoint: IEndpoint, getVariable: (name: string) => Variable): string {
    let path = '';

    function buildPath(endpoint: IEndpoint): void {
        path = `/${endpoint.directory.replace(/^:/, '')}${path}`;

        if (endpoint.type == "variable") {
            const variable = getVariable(endpoint.label);
            if (variable.value) {
                path = path.replace(endpoint.directory, variable.value.toString());
            }
        }
        // if (endpoint.directory.startsWith(':')) {
        //     const paramName = endpoint.directory.substring(1);
        //     if (params[paramName]) {
        //         path = path.replace(endpoint.directory, params[paramName].toString());
        //     }
        // }

        if (endpoint.children.length > 0) {
            endpoint.children.forEach(buildPath);
        }
    }

    buildPath(endpoint);
    return `/unity${path}`;
}
    // useEffect(() => {
    //     const newPath = [...path, endpoint.label];

    //     if (selectedVariable) {
    //         newPath[newPath.length - 1] = selectedVariable.value;
    //     }

    //     onPathUpdate(newPath);

    //     // Clean up function that removes the last item from the path when this component unmounts
    //     return () => {
    //         onPathUpdate(path);
    //     }
    // }, [selectedVariable]);

    // useEffect(() => {
    //     if (selectedEndpoint && selectedVariable) {
    //         onPathUpdate([...path, selectedEndpoint.label]);
    //     }
    // }, [selectedEndpoint])
