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

            if (message.StartsWith("{"))
            {
                try
                {
                    var packet = JsonUtility.FromJson<MessagePacket>(message);
                    if (packet.type == "status")
                    {
                        string newStatus = JsonUtility.ToJson(packet.data);
                        if (newStatus != lastStatusMessage)
                        {
                            manager.data = packet.data;

                            lastStatusMessage = newStatus;

                            Debug.Log("Tablero actualizado: " + string.Join(",", manager.data.board));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Error al leer JSON: " + ex.Message);
                }
            }
            else
            {
                string[] parts = message.Split('|');
                string code = parts[0];
                string content = parts.Length > 1 ? parts[1] : "";

                switch (code)
                {
                    case "501":
                        manager.winner.text = content;
                        break;

                    case "503":
                        if (content == "REPLAY ACCEPTED")
                        {
                            Debug.Log("Reiniciando juego...");
                            manager.ResetGame();
                        }
                        else if (content == "GAME CLOSED")
                        {
                            Debug.Log("El oponente terminó el juego");
                        }
                        break;

                    case "502":
                        Debug.Log("Revancha enviada");
                        break;

                    case "500":
                        Debug.Log("Partida iniciada");
                        break;

                    case "401":
                        Debug.Log("Invitación recibida de: " + content);
                        break;

                    default:
                        Debug.Log("Mensaje: " + message);
                        break;
                }
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
            await websocket.SendText("200|" + name);
        }
    }

    public async void SendMove(int box)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("501|" + box);
        }
    }

    public async void RequestGameStatus()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("300");
        }
    }

    public async void NewGame()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("503|YES");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
