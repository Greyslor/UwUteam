# UwUteam

## Diccionario de Comandos

- Case 200 (Referencia al nombre): 
  1. C: 200|nombre
     - _Envía solicitud de juego con tu propio nombre_
  1. S: 200|OKAY
     - _Acepta el nombre de usuario_
  1. S: 200|NO
     - _Niega el nombre de usuario_
- Case 300 (usuarios): 
  1. S: 300|[_array_]
     - _Servidor devuelve la lista de jugadores_
- Case 400 (Conecta a usuarios): 
  1. C: 401|nombreOtroPlayer
     - _Cliente selecciona el nombre del usuario para jugar_
  1. S: 401|Awaiting response
     - _Esperando la respuesta del otro jugador_
  1. C: 402|NO
     - _Niega la solicitud de juego del usuario_
  1. C: 402|YES
     - _Acepta la solicitud de juego del usuario_
  1. S: 404|User not found
     - _El usuario no fue encontrado_
- Case 500 (Juego): 
  1. S: 500|
     - _Estado de juego_
  1. C: 501|0 (número de 0 al 8)
     - _El jugador lanza el número de casilla_
  1. S: 502|
     - _Envía a los usuarios de vuelta al lobby_
  1. S: 503|(ganador player 1 o 2)
     - _Define al ganador de la partida_
