import { useContext, useEffect } from 'react'
import './App.css'

import { MIDINoteMonitor } from './components/MIDINoteMonitor';
import { MIDIControlMonitor } from './components/MIDIControlMonitor';
import { EndpointList } from './components/EndpointList';
import { OSCEndpointRouter } from './components/OSCEndpointRouter';
import { Navigation } from './layouts/navigation';
import { VariableDisplay } from './components/VariableDisplay';

import { GetVariable } from './context/VariableContext';

function App() {

  const getVariable = useContext(GetVariable);

  useEffect(() => {
    getVariable("artworkId").setValue("69");
    getVariable("foo").setValue("420");
    getVariable("bar").setValue("1337");
  }, [getVariable]);

  return (
    <div className='app'>
      <Navigation/>
      <div className='app-section'>
        <div style={{display: "flex", gap: "10px", flexWrap: "wrap"}}>
          <MIDINoteMonitor/>
          <MIDIControlMonitor/>

        </div>

      </div>
      <div className='app-section'>
        {/* <OSCEndpointRouter/> */}
        <VariableDisplay/>
      </div>
      {/* <OscMessager/> */}
      
      <div className='app-section'>
        <div className='flex-cards-container'>
          <EndpointList/>
        </div>
      
      </div>
      {/* <div style={{display: "flex", margin: "10px", gap: "10px", flexWrap: "wrap"}}>
      </div> */}
    </div>
  )
}

export default App
