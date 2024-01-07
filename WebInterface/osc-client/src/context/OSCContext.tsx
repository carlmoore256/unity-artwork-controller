import { createContext, useState, useEffect } from 'react';
import OSC from 'osc-js';

export interface OSCConnectionParameters {
    port : number;
    host : string;
}

export const OSCContext = createContext<OSC | null>(null);

export interface OSCProviderProps {
    oscConnectionParameters : OSCConnectionParameters;
    children : any;
}

export const OSCProvider = (props : OSCProviderProps) => {
    const { oscConnectionParameters, children } = props;
    const [osc, setOSC] = useState<OSC | null>(null);

    useEffect(() => {
        const osc = new OSC();
        osc.open({ port: oscConnectionParameters.port }); // check to see if host works, cause last
        // time it didn't

        // osc.open({ port: oscConnectionParameters.port, host : oscConnectionParameters.host });
        console.log(`osc opened on port ${oscConnectionParameters.port}`);
        setOSC(osc);
    }, []);

    return (
        <OSCContext.Provider value={osc}>
            {children}
        </OSCContext.Provider>
    )
}

