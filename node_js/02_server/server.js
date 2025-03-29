console.log("Hola mundo");
const express = require('express');
const app = express();
const port = 6060;

let board = [0,0,0,0,0,0,0,0,0];
let turn = 0;
let round = 0;
let score1 = 0;
let score2 = 0;

app.get('/', (req, res) =>{
    res.send('gatoxd');
});

app.get('/action/init', (req, res) =>{

    board = [0,0,0,0,0,0,0,0,0];
    turn = 0;
    round = 0;
    score1 = 0;
    score2 = 0;
    res.send('Inicialización de gato...');
});

app.get('/action/status/', (req, res) =>{
    let json = {"board" : board,
        "actual" : turn,
        "round" : round,
        "score1" : score1,
        "score2" : score2,
    }
    res.send(JSON.stringify(json));
});

app.get('/action/new_game/', (req, res) =>{
    board = [0,0,0,0,0,0,0,0,0];
    turn = 0;
    res.send('New game');
});

app.get('/action/turn/:player/:pos', (req, res) =>{
    let player;
    let pos = parseInt(req.params['pos']);
    switch (req.params['player']) {
        case "1":
            player = 1;
            break;
        case "2":
            player = 2;
            break;
        default:
            player = "error";
            break;
    };

    if (Number.isInteger(pos) &&pos > 0 && pos < 10 && Number.isInteger(player)){
        if(turn%2 == player % 2){
            res.send('jaja no es tu turno menso');
        }else{
            if(board[pos-1] !=0){
                res.send('ta ocupao');
            }else{
                board[pos-1] = player;
                turn++;
                if(board[0] == board[1] && board[1] == board[2]  && board[0] != 0||
                    board[3] == board[4] && board[4] == board[5] && board[3] != 0||
                    board[6] == board[7] && board[7] == board[8] && board[6] != 0||
                    board[0] == board[3] && board[3] == board[6] && board[0] != 0||
                    board[1] == board[4] && board[4] == board[7] && board[1] != 0||
                    board[3] == board[5] && board[5] == board[8] && board[3] != 0||
                    board[0] == board[4] && board[4] == board[8] && board[0] != 0||
                    board[2] == board[4] && board[4] == board[6] && board[2] != 0
                ){
                    if(turn%2 == 0){
                        res.send('gano player2');
                        round++;
                        score2++;
                    }else{
                        res.send('gano player1');
                        round++;
                        score1++;
                    }
                }else if(turn == 9){
                    res.send('empate unu')
                }else{
                    res.send(player + ' ha tirado en la posición' + pos + 'tablero:' + board);
                }
            }
        }
    }else{
        res.send('ERROR');
    }
});

app.listen(port,() =>{
    console.log(`server init: ${port}`);
});