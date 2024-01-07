export interface IVariableListener {
    name : string;
    onValueChanged : (value : any) => void;
}

export class Variable {

    name : string;
    value : any;
    listeners : IVariableListener[] = [];

    constructor(name : string, value : any) {
        this.name = name;
        this.value = value;
    }

    setValue(value : any) {
        this.value = value;
        this.listeners.forEach(listener => {
            listener.onValueChanged(value);
        });
    }

    setName(name : string) {
        this.name = name;
    }
    
    addListener(listener : IVariableListener) {
        this.listeners.push(listener);
    }
}
