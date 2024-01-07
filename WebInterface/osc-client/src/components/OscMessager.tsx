import { useContext, useEffect } from 'react';
import '../App.css'
import { Card } from "@blueprintjs/core";
import OSC from 'osc-js';

const PORT = 8080;
const osc = new OSC();
osc.open({ port: PORT });
console.log(`osc opened on port ${PORT}`);


export function OscMessager() {

    function handleOsc() {
      var message = new OSC.Message('/test/random', Math.random());
      osc.send(message);
    }

    return (
        <>
            <Card>
                <button onClick={handleOsc}>
                Send OSC Message
                </button>
            </Card>
        </>
  )
}