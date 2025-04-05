using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

[Serializable]
public class GameMessage
{
    public string type;
    public int status;
    public string[] board;
    public string turn;
    public string winner;
}

public class WebSocketManager : MonoBehaviour
{
    WebSocket websocket; //conetzion al servidor

    public string playerId = "jugador1";
    public GameManager gameManager;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Conectado!");
            SendJoinMessage();
        };

        websocket.OnMessage += (bytes) =>
        {
            // Convertir los bytes recibidos en un string usando UTF-8
            string json = Encoding.UTF8.GetString(bytes);
            Debug.Log("Mensaje recibido: " + json);

            // Convierte el JSON en un objeto GameMessage usando JsonUtility
            GameMessage msg = JsonUtility.FromJson<GameMessage>(json);

            if (msg.type == "update")
            {
                gameManager.UpdateBoard(msg);
            }
        };

        websocket.OnError += (e) => Debug.Log("Error: " + e);
        websocket.OnClose += (e) => Debug.Log("Conexión cerrada");

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void SendJoinMessage()
    {
        string json = $"{{\"type\":\"join\",\"player\":\"{playerId}\"}}";
        await websocket.SendText(json);
    }

    public async void SendMove(int box)
    {
        string json = $"{{\"type\":\"move\",\"player\":\"{playerId}\",\"box\":{box}}}";
        await websocket.SendText(json);
    }

    async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
