const WebSocket = require('ws');
const server = new WebSocket.Server({ port: 8080 });

let clients = {};
let board = ["", "", "", "", "", "", "", "", ""];
let turn = "jugador1";

function checkWinner() {
  const combos = [
    [0,1,2], [3,4,5], [6,7,8],
    [0,3,6], [1,4,7], [2,5,8],
    [0,4,8], [2,4,6]
  ];
  for (let c of combos) {
    const [a, b, c2] = c;
    if (board[a] && board[a] === board[b] && board[a] === board[c2]) {
      return board[a];
    }
  }
  return board.includes("") ? null : "empate";
}

function broadcastGameState() {
  const winner = checkWinner();
  const state = {
    type: "update",
    status: 200,
    board,
    turn,
    winner
  };
  const message = JSON.stringify(state);
  for (const id in clients) {
    clients[id].send(message);
  }
}

server.on('connection', socket => {
  let playerId = null;

  socket.on('message', data => {
    try {
      const msg = JSON.parse(data);

      if (msg.type === "join") {
        playerId = msg.player;
        clients[playerId] = socket;
        console.log(playerId + " conectado");
        broadcastGameState();
      }

      if (msg.type === "move" && msg.player === turn) {
        const box = msg.box;
        if (board[box] === "") {
          board[box] = msg.player;
          turn = (turn === "jugador1") ? "jugador2" : "jugador1";
          broadcastGameState();
        } else {
          socket.send(JSON.stringify({ type: "error", status: 203, message: "Casilla ocupada" }));
        }
      }
    } catch (e) {
      console.error("Error al procesar mensaje:", e);
    }
  });

  socket.on('close', () => {
    console.log(playerId + " desconectado");
    delete clients[playerId];
  });
});

console.log("Servidor WebSocket corriendo en ws://localhost:8080");
