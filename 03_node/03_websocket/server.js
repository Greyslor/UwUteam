const WebSocket = require('ws');
const clients = [];

class Cliente {
	constructor(ws) {
		this.ws = ws;
		this.username = "No name";
		this.id = null; // "1" o "2"
	}
}

let gameData = {
	board: Array(9).fill(0),
	actual: 1,
	round: 1,
	score1: 0,
	score2: 0
};

const wss = new WebSocket.Server({ port: 8080 }, () => {
	console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	console.log('New connection');
	let cliente = new Cliente(ws);
	clients.push(cliente);

	// Asignar ID de jugador
	const currentPlayers = clients.filter(c => c.id !== null).length;
	if (currentPlayers < 2) {
		cliente.id = (currentPlayers + 1).toString();
	}

	ws.on('message', (data) => {
		const message = data.toString();
		console.log("Received:", message);

		try {
			const json = JSON.parse(message);

			switch (json.type) {
				case 'set_username':
					cliente.username = json.username;
					broadcast({ type: "system", message: `${cliente.username} se ha unido.` });
					break;

				case 'chat':
					broadcast({ type: 'chat', username: cliente.username, message: json.message });
					break;

				case 'new_game':
					resetGame();
					broadcastGameData();
					break;

				case 'turn':
					handleTurn(cliente.id, json.box);
					break;

				case 'get_status':
					if (gameData.actual.toString() !== cliente.id) {
            sendToClient(cliente, { type: "status", data: gameData });
          }
          break;
			}
		} catch (err) {
			console.error("Invalid JSON:", err);
		}
	});

	ws.on('close', () => {
		const index = clients.indexOf(cliente);
		if (index > -1) clients.splice(index, 1);
		console.log(`${cliente.username} disconnected`);
	});
});

function handleTurn(playerId, box) {
	box = parseInt(box);
	if (gameData.board[box] === 0 && gameData.actual.toString() === playerId) {
		gameData.board[box] = parseInt(playerId);
		gameData.actual = gameData.actual === 1 ? 2 : 1;

		// verificar si hay ganador
		if (checkWinner(parseInt(playerId))) {
			if (playerId === "1") gameData.score1++;
			else gameData.score2++;

			broadcast({"type":"winner","message":playerId});

			//gameData.round++;
			//gameData.board = Array(9).fill(0);
		}

		broadcastGameData();
	}
}

function checkWinner(player) {
	const wins = [
		[0, 1, 2], [3, 4, 5], [6, 7, 8],
		[0, 3, 6], [1, 4, 7], [2, 5, 8],
		[0, 4, 8], [2, 4, 6]
	];

	return wins.some(combo => combo.every(i => gameData.board[i] === player));
}

function resetGame() {
	gameData = {
		board: Array(9).fill(0),
		actual: 1,
		round: 1,
		score1: 0,
		score2: 0
	};
}

function broadcast(data) {
	const str = JSON.stringify(data);
	clients.forEach(c => {
		if (c.ws.readyState === WebSocket.OPEN) {
			c.ws.send(str);
		}
	});
}

function broadcastGameData() {
	broadcast({ type: "status", data: gameData }); 
}

function sendToClient(cliente, data) {
	if (cliente.ws.readyState === WebSocket.OPEN) {
		cliente.ws.send(JSON.stringify(data));
	}
}
