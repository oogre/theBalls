/*----------------------------------------*\
  Super Easy Terminal Chat Server - index.js
  @Desc : Chat server, it broadcasts all incomming messages to all connected clients 
  @git : 
  @author Evrard Vincent (vincent@ogre.be)
  @Date:   2019-10-01 
  @Last Modified time: 2020-01-03 03:15:53
  @Dependencie : https://github.com/websockets/ws
\*----------------------------------------*/

process.title = 'server.theballs';

const os = require("os");
const WebSocket = require('ws');
const port = process.env.PORT || 1337 ;

const wss = new WebSocket.Server({ port: port });

//https://github.com/websockets/ws#simple-server
//https://github.com/websockets/ws#server-broadcast
wss.on('connection', function connection(ws) {
	ws.on('message', function incoming(data) {
		wss.clients.forEach(function each(client) {
			if (client !== ws && client.readyState === WebSocket.OPEN) {
				client.send(data);
			}
		});
	});
});

console.log("Ready to get request at : ");
console.log("HOST : " + os.hostname());
console.log("PORT : " + port);
console.log("\t ex : wsc ws://" + os.hostname() + ":" + port);
