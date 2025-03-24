console.log("Hola mundo");
const express = require('express');
const app = express();
const port = 6060;
var count=0;

app.get('/', (req, res) =>{
    res.send('Hello world');
});

app.get('/action/init', (req, res) =>{
    res.send('Inicialización de gato...');
});
app.get('/count', (req, res) =>{
    count++;
    res.send('Contador' + count);
});


app.get('/action/status/:player', (req, res) =>{
    res.send('Return status of player' + req.params["player"] + '<br>Contador:' + count);
});

app.get('/action/turn/:player/:pos', (req, res) =>{
    let player = "";
    let pos = req.params['pos'];
    switch (req.params['player']) {
        case "1":
            player = "player01";
            break;
        case "2":
            player = "player02";
            break;
        default:
            player = "error";
            break;
    };
    if (pos > 0){
        
    }
    res.send(player + ' ha tirado en la posición' + pos);
});

app.listen(port,() =>{
    console.log(`server init: ${port}`);
});

