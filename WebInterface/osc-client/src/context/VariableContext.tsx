import { createContext, useState, useEffect, useCallback, useContext } from 'react';
import { Variable } from '../data/Variable';

export const GetVariable = createContext<((name: string) => Variable)>(() => new Variable('null', null));
export const SetVariable = createContext<((name: string, value: any) => void)>(() => {});
export const AddVariable = createContext<((v: Variable) => void)>(() => {});
export const AllVariables = createContext<Variable[] | null>(null);


export interface IVariable {
    name: string;
    value: any;  // or a more specific type if you know what types of values your variables can have
}

export interface IVariableContext {
    variables: Record<string, IVariable>;
    setVariable: (name: string, value: any) => void;
}
  
export const VariableContext = createContext<IVariableContext>({
    variables: {
        "myVariable": {
            name: "myVariable",
            value: 10,
        }
    },
    setVariable: () => {},
});

// Define the provider
export const VariableProvider = (props : { children : any }) => {
    const { children } = props;
    const [variables, setVariables] = useState<Record<string, IVariable>>({});

    useEffect(() => {
        console.log("variables changed", variables);
    }, [variables]);

    const setVariable = (name : string, value : any) => {
        console.log("setVariable", name, value);
        setVariables(oldVariables => ({
            ...oldVariables,
            [name]: value,
        }));
    };

    return (
        <VariableContext.Provider value={{ variables, setVariable }}>
            {children}
        </VariableContext.Provider>
    );
};
  


// Define a custom hook for accessing the variable context
// export const useVariable = (name : string) => {
//     const { variables, setVariable } = useContext<IVariableContext>(VariableContext);
    
//     useEffect(() => {
//         console.log("useEffect", name, variables[name]);
//       // Here you could add your IVariableListener logic,
//       // perhaps by invoking some onValueChanged callback
//     }, [variables[name]]);
    
//     return [variables[name], (newValue : any) => setVariable(name, newValue)];
// };


export const useVariable = (name: string): [IVariable, (newValue: any) => void] => {
    const { variables, setVariable } = useContext(VariableContext);
    const variable = variables[name];

    const setVarValue = (newValue: any) => {
        if(variable) {
            setVariable(name, newValue);
        }
    }

    return [variable, setVarValue];
};



















// export const VariableProvider = (props : any) => {
    
//     const [variables, setVariables] = useState<Variable[]>([]);

//     const getVariable = useCallback((name : string) => {
//         const variable = variables.find(v => v.name === name);
//         if (variable) {
//             return variable;
//         }
//         const newVariable = new Variable(name, null);
//         setVariables([...variables, newVariable]);
//         console.log("new variable created", name);
//         return newVariable;
//     }, [variables]);

//     const setVariable = useCallback((name : string, value : any) => {
//         const variable = variables.find(v => v.name === name);
//         if (variable) {
//             variable.value = value;
//             setVariables([...variables]);
//         } else {
//             const newVariable = new Variable(name, value);
//             setVariables([...variables, newVariable]);
//         }
//     }, [variables]);

//     const addVariable = useCallback((v : Variable) => {
//         setVariables([...variables, v]);
//     }, [variables]);
    
//     return (
//         <>
//             <GetVariable.Provider value={getVariable}>
//                 <AllVariables.Provider value={variables}>
//                     <SetVariable.Provider value={setVariable}>
//                         <AddVariable.Provider value={addVariable}>
//                             {props.children}
//                         </AddVariable.Provider>
//                     </SetVariable.Provider>
//                 </AllVariables.Provider>
//             </GetVariable.Provider>
//         </>
//     );
// }