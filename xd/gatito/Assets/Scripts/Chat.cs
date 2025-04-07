using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

[Serializable]
public class MessagePacket
{
    public string type;
    public string username;
    public string message;
    public int box;
    public GatoData data;
}

public class Chat : MonoBehaviour
{
    WebSocket websocket;
    public string playerName = "Vlad";
    public GameManager manager;

    private string lastStatusMessage = "";

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Conectado");
            SendUsername(playerName);
        };

        websocket.OnError += (e) => Debug.Log("Error: " + e);

        websocket.OnClose += (e) => Debug.Log("Desconectado");

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Recibido: " + message);

            var packet = JsonUtility.FromJson<MessagePacket>(message);

            switch (packet.type)
            {
                case "chat":
                    Debug.Log($"{packet.username}: {packet.message}");
                    break;
                case "status":
                    // Verificar si el estado ha cambiado antes de mostrarlo
                    string newStatus = JsonUtility.ToJson(packet.data);
                    if (newStatus != lastStatusMessage)
                    {
                        Debug.Log("Recibido nuevo estado: " + newStatus);
                        manager.data = JsonUtility.FromJson<GatoData>(newStatus);
                        lastStatusMessage = newStatus;  // Actualizar el último estado
                    }
                    break;
                case "system":
                    Debug.Log("System message: " + packet.message);
                    break;
                case "winner":
                    manager.winner.text = "Gana Player " + packet.message;
                    break;

            }
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public async void SendChatMessage(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            var json = JsonUtility.ToJson(new MessagePacket
            {
                type = "chat",
                username = playerName,
                message = msg
            });
            await websocket.SendText(json);
        }
    }

    public async void SendUsername(string name)
    {
        if (websocket.State == WebSocketState.Open)
        {
            var json = JsonUtility.ToJson(new MessagePacket
            {
                type = "set_username",
                username = name
            });
            await websocket.SendText(json);
        }
    }

    public async void SendMove(int box)
    {
        if (websocket.State == WebSocketState.Open)
        {
            var json = JsonUtility.ToJson(new MessagePacket
            {
                type = "turn",
                box = box
            });
            await websocket.SendText(json);
        }
    }

    public async void RequestGameStatus()
    {
        if (websocket.State == WebSocketState.Open)
        {
            var json = JsonUtility.ToJson(new MessagePacket
            {
                type = "get_status"
            });
            await websocket.SendText(json);
        }
    }

    public async void NewGame()
    {
        if (websocket.State == WebSocketState.Open)
        {
            var json = JsonUtility.ToJson(new MessagePacket
            {
                type = "new_game"
            });
            await websocket.SendText(json);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
