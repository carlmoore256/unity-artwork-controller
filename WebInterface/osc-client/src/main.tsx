import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './index.css';
import { MIDIProvider } from './context/MIDIContext.tsx';
import { OSCProvider } from './context/OSCContext.tsx';
import { VariableProvider } from './context/VariableContext.tsx';

const DEFAULT_OSC_HOST = "localhost";
const DEFAULT_OSC_PORT = 8080;

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <MIDIProvider>
      <OSCProvider oscConnectionParameters={{port : 8080, host : "localhost"}}>
        <VariableProvider>
          <App />
        </VariableProvider>
      </OSCProvider>
    </MIDIProvider>
  </React.StrictMode>,
)
