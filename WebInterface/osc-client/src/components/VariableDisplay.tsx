import { useEffect, useState, useContext, useCallback } from "react";
import { Card, Button, Intent, EditableText, H5 } from "@blueprintjs/core";
import { Variable, IVariableListener } from "../data/Variable";
import { useVariable, VariableContext, IVariableContext} from "../context/VariableContext";

export function VariableDisplay(props : {}) {

    const [addButtonIntent, setAddButtonIntent] = useState<Intent>("primary");

    const { variables, setVariable } = useContext<IVariableContext>(VariableContext);
    // const [myVariable, setMyVariable] = useVariable("myVariable");

    // const [variables, setVariables] = useState<Variable[]>([]);
    // const allVariables = useContext(AllVariables);
    // const getVariable = useContext(GetVariable);
    // const setVariable = useContext(SetVariable);
    // const addVariable = useContext(AddVariable);



    const handleAddVariable = useCallback((name : string) => {
        while (variables[name]) {
            name = name + "_copy";
        }
        console.log("Gonna set variable ", name);
        setVariable(name, null);
    }, [variables]);
    
    return (
        <>
            <Card>
                <H5>Variables</H5>

                <div style={{display: "flex", gap: "5px", margin: "10px", flexWrap: "wrap"}}>
                    {/* {Object.entries(variables).map(([key, value]) => {
                        <VariableCard key={variable.name} variable={variable} />
                    }} */}
                    {Object.entries(variables).map(([key, value]) => (
                        <VariableCard key={key} name={key} />
                    ))}
                </div>
                
                <Button 
                    onClick={() => handleAddVariable(`variable_${variables?.length}`)}
                    intent={addButtonIntent}
                >
                    Add Variable
                </Button>
                {/* </Button> */}
            </Card>
                
        </>
    );
}


export function VariableCard (props : { name : string }) {
    const { name } = props;

    console.log("NAME IS ", name);

    const [variable, setVariable] = useVariable(name);
    const [value, setValue] = useState<any>("");

    // Sync value state with variable value
    useEffect(() => {
        console.log("variable changed", variable);
        setValue(variable.value);
    }, [variable]);

    // Handle text input changes
    const handleValueChange = (newValue: string) => {
        setValue(newValue);
        variable.value = newValue;
        setVariable(variable.value);
    };

    // Handle name changes
    const handleNameChange = (newName: string) => {
        variable.name = newName;
        // Here you should also update the variable in your VariableContext accordingly
    };

    return (
        <Card style={{background: "limegreen", padding: "8px"}}>
            <H5>
                <EditableText 
                    value={variable && variable.name}
                    onConfirm={handleNameChange}
                />
            </H5>
            <EditableText
                value={value}
                onChange={handleValueChange}
                onConfirm={handleValueChange}
            />
        </Card>
    )
}



// export function VariableCard (props : { name : string }) {
//     const { name } = props;

//     const [variable, setVariable] = useVariable(name);
//     const [value, setValue] = useState<any>("");

//     // const [variable, setVariable] = useState<Variable>(new Variable('null', null));

//     useEffect(() => {
//         console.log("variable changed", variable);
        
//         setValue(variable.value);

//     }, [variable]);
    
//     useEffect(() => {
//         setVariable();
//     }, [value]);


//     return (
//         <>
//             <Card style={{background: "limegreen", padding: "8px"}}>
//                 <H5>
//                     <EditableText 
//                         value={variable.name}
//                         onConfirm={(value) => {
//                             variable.name = value;
//                         }}
//                     />
//                 </H5>
//                 <EditableText
//                     value={value}
//                     onChange={(value) => {
//                         setValue(value);
//                     }}
//                     onConfirm={(value) => {
//                         setValue(value);
//                     }}
//                 />
//             </Card>
//         </>
//     )
// }



        
        // const testListener : IVariableListener = {
        //     name : "Foo Bar",
        //     onValueChanged: (value) => {
        //         console.log("variable value changed", value);
        //     }
        // }
        // variable.addListener(testListener);