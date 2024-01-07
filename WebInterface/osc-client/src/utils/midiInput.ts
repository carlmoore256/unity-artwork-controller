import { EndpointConnection } from "../components/EndpointConnection";

export function tryApplyProcessor(endpoint : EndpointConnection, value : number | null = null) {
    var output = null;

    if (value == null && !endpoint.message) {
        console.error("No value to process");
        return;
    }

    if (value == null) {
        // @ts-ignore
        value = endpoint.message.value;
    }

    if (!endpoint.processor) {
        return value;
    }
    
    try {
        const func = eval(endpoint.processor);
        output = func(value);
    } catch (e) {
        console.error(e);
        return value;
    }
    
    if (output == null) {
        console.error("Processor returned null " + endpoint.processor);
        return value;
    }

    return output;
}