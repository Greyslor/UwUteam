using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Info
{
    public string id;
    public string message;
}
public class GameManagerg2 : MonoBehaviour
{
    [Header("Connection")]
    WebSocket websocket;
    public string playerName = "";
    public GatoData data;

    [Header("UI")]
    public GameObject userInput;
    public TextMeshProUGUI usernameInput;
    public GameObject userInvalidPanel;

    public GameObject userlistPanel;
    public TMP_Dropdown UserList;
    public TextMeshProUGUI UserListText;
    public List<string> UserListNames;
    public GameObject awaitingPanel;

    public GameObject requestpanel;
    public TextMeshProUGUI requestuser;
    public string requestingPlayer;

    public GameObject gamepanel;
    public TextMeshProUGUI winner;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Conectado");
        };

        websocket.OnError += (e) => Debug.Log("Error: " + e);

        websocket.OnClose += (e) => Debug.Log("Desconectado");

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes).Split('|');
            Debug.Log(message[0]);
            switch (message[0])
            {
                case "200":
                    if(message[1] == "NO")
                    {
                        userInvalidPanel.SetActive(true);
                    }
                    else if(message[1] == "OKAY")
                    {
                        RequestUsers();
                        userInput.SetActive(false);
                        userlistPanel.SetActive(true);
                    }
                    break;

                case "300":
                    UserListNames.Clear();
                    UserList.ClearOptions();
                    foreach(var us in message[1].Split(","))
                    {
                        UserListNames.Add(us);
                    }
                    UserList.AddOptions(UserListNames);
                    break;

                case "401":
                    requestingPlayer = message[1];
                    requestuser.text = requestingPlayer;
                    requestpanel.SetActive(true);
                    userlistPanel.SetActive(false);
                    break;
                case "402":
                    awaitingPanel.SetActive(false);
                    if (message[2] == "YES")
                    {
                        userlistPanel.SetActive(false);
                        gamepanel.SetActive(true);
                        websocket.SendText("500|");
                    }
                    break;
                case "500":
                    data = JsonUtility.FromJson<GatoData>(message[1]);
                    break;
                case "504":
                    RequestUsers();
                    break;
                case "502":
                    StartCoroutine(BackToLobby());
                    break;
                case "503":
                    winner.text = "PLAYER "+message[1]+" WINS";
                    break;
                case "505":
                    websocket.SendText("500|");
                    break;
                case "404":
                    awaitingPanel.SetActive(false);
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

    public async void ChangeUser()
    {
        playerName = usernameInput.text;

        if (websocket.State == WebSocketState.Open)
        {
            var message = "200|"+playerName;
            await websocket.SendText(message);
        }
    }
    public async void RequestUsers()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("300|");
        }
    }

    public async void RequestGame()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("401|"+UserListText.text);
            awaitingPanel.SetActive(true);
        }
    }
    public async void ResponseGame(string response)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("402|" + requestingPlayer +"|"+response);
            if (response == "NO")
            {
                requestpanel.SetActive(false);
                userlistPanel.SetActive(true);
            }
            else if(response == "YES")
            {
                requestpanel.SetActive(false);
                gamepanel.SetActive(true);
                await websocket.SendText("500|");
            }
        }
    }
    public async void Play(string pos)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("501|" + pos);
        }
    }
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void Return()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("502|");
        }
    }

    public IEnumerator BackToLobby()
    {
        yield return new WaitForSeconds(5);
        Return();
        data = new GatoData();
        winner.text = "";
        userlistPanel.SetActive(true);
        gamepanel.SetActive(false);
    }
}
