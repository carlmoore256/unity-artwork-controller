const OSC = require('osc-js')

const SEND_PORT = 8000;

const config = { udpClient: { port: SEND_PORT } }
const osc = new OSC({ plugin: new OSC.BridgePlugin(config) })

osc.open() // start a WebSocket server on port 8080

osc.on('open', () => {
    console.log(`OSC Bridge is listening for messages, sending to port ${SEND_PORT}`)
})