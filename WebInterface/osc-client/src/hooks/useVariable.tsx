import React, { createContext, useContext, useEffect, useState } from "react";

// Define a custom hook for accessing the variable context
export const useVariable = (name : string) => {
    const { variables, setVariable } = useContext(VariableContext);
    
    useEffect(() => {
      // Here you could add your IVariableListener logic,
      // perhaps by invoking some onValueChanged callback
    }, [variables[name]]);
    
    return [variables[name], (newValue) => setVariable(name, newValue)];
  };