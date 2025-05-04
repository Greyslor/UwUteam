const WebSocket = require('ws');
const uss = [];
const users = [];
const activeGames = [];

class Game{
	constructor()
	{
		this.board=[0,0,0,0,0,0,0,0,0];
		this.round=1;
		this.turn=1;
		this.score1=0;
		this.score2=0;
		this.player1="";
		this.player2="";
		this.roundEnded=false;
		this.replay1="";
		this.replay2="";
	}
}

class User{
	constructor()
	{
		this._username="none";
		this._player = 0;
		this._inGame = false;
		this._conn = null;
	}

	set player ( user )
	{
		this._player = user;
	}

	get player ()
	{
		return this._player;
	}

	set inGame ( state )
	{
		this._inGame = state;
	}

	get inGame ()
	{
		return this._inGame;
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

function checkwin( game, users){
	if(game.board[0] == game.board[1] && game.board[1] == game.board[2] && game.board[0] != 0 ||
		game.board[3] == game.board[4] && game.board[4] == game.board[5] && game.board[3] != 0 ||
		game.board[6] == game.board[7] && game.board[7] == game.board[8] && game.board[6] != 0 ||
		game.board[0] == game.board[3] && game.board[3] == game.board[6] && game.board[0] != 0 ||
		game.board[1] == game.board[4] && game.board[4] == game.board[7] && game.board[1] != 0 ||
		game.board[2] == game.board[5] && game.board[5] == game.board[8] && game.board[2] != 0 ||
		game.board[0] == game.board[4] && game.board[4] == game.board[8] && game.board[0] != 0 ||
		game.board[2] == game.board[4] && game.board[4] == game.board[6] && game.board[2] != 0){
			if(game.turn%2 == 0){
				game.score1 += 1;
				users.forEach(us => {
					if(us.username === game.player1 || us.username === game.player2)
					{
						game.roundEnded = true;
						us.connection.send("501|Player 1 wins");
						us.connection.send("502|REPLAY");
					}
				});
			}
			else{
				game.score2 += 1;
				users.forEach(us => {
					if(us.username === game.player1 || us.username === game.player2)
					{
						game.roundEnded = true;
						us.connection.send("501|Player 2 wins");
						us.connection.send("502|REPLAY");
					}
				});
			}
	}
	else if(game.turn > 9){
		users.forEach(us => {
			if(us.username === game.player1 || us.username === game.player2)
			{
				us.connection.send("501|Draw");
			}
		});
	}
}

function checkturn(game, user, pos, users){
	if(game.turn % 2 == user.player % 2){
		if(pos > -1 && pos < 9) {
			if(game.board[pos] == 0){
				game.board[pos] = user.player;
				game.turn += 1;
			}
			else{
				user.connection.send("501|invalid move");
			}
		}
		else{
			user.connection.send("501|invalid move");
		}
	}
	else {
		user.connection.send("501|not your turn");
	}
	sendGameStatus(game, users);
}

function sendGameStatus(game, users) {
	const message = JSON.stringify({
		type: "status",
		data: {
			board: game.board,
			score1: game.score1,
			score2: game.score2,
			round: game.round,
			actual: game.turn % 2 === 1 ? 1 : 2
		}
	});

	users.forEach(us => {
		if (us.username === game.player1 || us.username === game.player2) {
			us.connection.send(message);
		}
	});
}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connection');
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
		let u=true;

		switch (info[0])
		{
			case '200':
	let found = false;
	users.forEach(us => {
		if(us.username === info[1])
		{
			found = true;
		}
	});

	if(!found)
	{
		user.username = info[1];
		user.connection.send("200|OKAY");

		// CREA JUEGO AUTOMÁTICO PARA 2 USUARIOS
		let p1 = users.find(u => u.username !== "none" && u.username !== user.username && !u.inGame);
		if (p1) {
			user.player = 2;
			p1.player = 1;
			user.inGame = true;
			p1.inGame = true;

			let game = new Game();
			game.player1 = p1.username;
			game.player2 = user.username;
			activeGames.push(game);

			p1.connection.send("500|game started");
			user.connection.send("500|game started");
			sendGameStatus(game, users);
		}
	}
	else{
		user.connection.send("200|NO")
	}
	break;

				
			case '300':
				let lista = [];
				users.forEach(us => {
					if(us.connection.readyState === WebSocket.OPEN)
					{
						if(!(us.username === "none") && !us.inGame && us.username != user.username){
							lista.push(us.username);
						}
					}
				});

				let json = "{\"Users\" : "+JSON.stringify(lista)+"}"

				user.connection.send(json);

				break;

			case '401':

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("401|" + user.username);
						user.connection.send("401|Awaiting response");

					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}
				break;

			case '402':
				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;

						if(info[2] == "YES"){
							user.player = 1;
							us.player = 2;
							user.inGame=true;
							us.inGame=true;
							let game = new Game();
							game.player1 = user.username;
							game.player2 = us.username;
							activeGames.push(game);
							us.connection.send("402|" + user.username + "|game accepted");
							us.connection.send("500|game started");
							user.connection.send("500|game started");
						}
						else if(info[2] == "NO"){
							us.connection.send("402|" + user.username + "|game denied");
						}
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}
				break;

			case '501':
				activeGames.forEach(gm => {
					if(gm.player1 == user.username || gm.player2 == user.username){
						checkturn(gm, user, parseInt(info[1]), users);
						checkwin(gm, users);
						sendGameStatus(gm, users);
					}
				});
				console.log(activeGames);

				break;

			case '503':
				activeGames.forEach(gm => {
					if(gm.player1 == user.username || gm.player2 == user.username){
						if(gm.player1 == user.username ){
							gm.replay1 == info[1]
							if(gm.replay1 == "YES"){
								if(gm.replay2 == "YES"){
									users.forEach(us => {
										if(us.username === gm.player1 || us.username === gm.player2)
										{
											us.connection.send("503|REPLAY ACCEPTED");
											if(us.player == 1){
												us.player = 2;
											}
											else if(us.player == 2){
												us.player = 1;
											}
										}
									});
									gm.board = [0,0,0,0,0,0,0,0,0];
									gm.round += 1;
									gm.roundEnded = false;
								}
								else if(gm.replay2 == ""){
									user.connection.send("503|awaiting response");
								}
							}
							else if(gm.replay1 == "NO"){
								users.forEach(us => {
									if(us.username === game.player1 || us.username === game.player2)
									{
										us.connection.send("503|GAME CLOSED");
										us.inGame = false;
										us.player = 0;
									}
								});
								activeGames.splice(activeGames.indexOf(gm), 1);
							}
						}
						else if(gm.player2 == user.username ){

						}
					}
				});
	
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