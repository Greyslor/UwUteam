const WebSocket = require('ws');
const clients = [];

class Cliente{
	constructor()
	{
		this._username="No name";
	}

	set username( user )
	{
		this._username = user;
	}

	get username ()
	{
		return this._username;
	}
}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
	clients.push(ws); // Agregar la conexión (cliente) a la lista

	let cliente = new Cliente ();
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s',data);
		
		//ws.send("The server response: "+data); // Para mandar el mensaje al cliente que lo envió

		let info = data.toString().split('|');

		switch (info[0])
		{
			case '200':
				cliente.username = info[1];
				ws.send("UserName upDated: "+cliente.username);
				break;
			
				default:
					// Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
					clients.forEach(client => {
						if(client.readyState === WebSocket.OPEN)
						{
							client.send(cliente.username + " says: " + data); // si falla, cambiar a: `data.toString()`
						}
					});
					break;
		}
	});

	// Al cerrar la conexión, quitar de la lista de clientes
	ws.on('close', () => { 
		let index = clients.indexOf(ws);
		if(index > -1)
		{
			clients.splice(index, 1);
			ws.send("UserName disconnected: "+cliente.username);
		}
	});
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});