const WebSocket = require('ws');
const uss = [];
const users = [];

class User{
	constructor()
	{
		this._username="No name";
		this._conn=null;
	}

	set username( user )
	{
		this._username = user;
	}

	get username ()
	{
		return this._username;
	}

	set connection( con )
	{
		this._conn = con;
	}

	get connection()
	{
		return this._conn;
	}

	static findClientByUsername (lst, username)
	{
		lst.forEach(user => {
			if(user.username === username)
			{
				return user;
			}
		});
		return null;
	}
}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
	let user = new User ();
	user.connection = ws;
	users.push(user); // Agregar la conexión (use) a la lista

	//let use = new use ();
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s',data);
		
		//ws.send("The server response: "+data); // Para mandar el mensaje al use que lo envió

		let info = data.toString().split('|');

		switch (info[0])
		{
			case '200':
				user.username = info[1];
				user.connection.send("200|UserName upDated: " +user.username);
				break;
			
			case '300':
				let lista = "";
				users.forEach(us => {
					if(us.connection.readyState === WebSocket.OPEN)
					{
						lista = lista + us.username + " - ";
					}
				});
				user.connection.send("300|list: "+ lista);

				break;

			case '400':
				let u=true;

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("400|" + user.username + info[2]);
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}
				break;

			case '404':
				break;

				default:
					// Mandar a todos los uses conectados el mensaje con el username de quien lo envió
					users.forEach(us => {
						if(us.readyState === WebSocket.OPEN)
						{
							us.send(use.username + " says: " + data); // si falla, cambiar a: `data.toString()`
						}
					});
					break;
		}
	});

	// Al cerrar la conexión, quitar de la lista de uses
	ws.on('close', () => { 
		let index = users.indexOf(user);
		if(index > -1)
		{
			uss.splice(index, 1);
			user.connection.send("UserName disconnected: "+user.username);
		}
	});
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});