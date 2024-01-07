import { useContext, useEffect, useState } from 'react';
import { HTMLSelect } from '@blueprintjs/core';
import { GetVariable, AllVariables } from '../context/VariableContext';
import { Variable } from '../data/Variable';

export function VariableDropdown(props : { setSelectedVariable : Function}) {

    const { setSelectedVariable } = props;
    // const [selectedVariable, setSelectedVariable] = useState<Variable | null>(null);

    const getVariable = useContext(GetVariable);
    const allVaraibles = useContext(AllVariables);

    return (
        <>
            <HTMLSelect 
                minimal={true} 
                style={{borderRadius: "0 10px 10px 0px", backgroundColor: "limegreen"}}
                onChange={(e) => {
                    setSelectedVariable(getVariable(e.currentTarget.value)
                )}}
            >
                {allVaraibles?.map(variable => (
                    <option key={variable.name} value={variable.name}>{variable.name}</option>
                ))}
            </HTMLSelect>
        </>
    )
}