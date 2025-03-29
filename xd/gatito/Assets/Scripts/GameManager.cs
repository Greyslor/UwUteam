using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [Header("Object references")]
    public GameObject playerSelection;
    public GameObject game;

    [Header ("Data Base")]
    public GatoData data;
    public string id;
    private int box;

    public delegate void GotData();
    public GotData onGotData;

    private bool canUpdate = true;
    void Start()
    {
        StopAllCoroutines();
        playerSelection.SetActive(true);
        game.SetActive(false);
    }

    public void SelectId(string idInput)
    {
        id = idInput;
        playerSelection.SetActive(false);
        game.SetActive(true);

    }

    private void Update()
    {
        if (canUpdate && id != string.Empty)
        {
            StartCoroutine(GetInfo());
        }
    }

    public void PlayerMove(int boxInput)
    {
        box = boxInput;
        StartCoroutine(SendMove());
    }

    [ContextMenu("newGame")]
    public void NewGameBtn()
    {
        StartCoroutine(NewGame());
    }

    [ContextMenu("resetgame")]
    public void ResetGame()
    {
        StartCoroutine(ResetInfo());
    }

    //Acciones en el php
    IEnumerator ResetInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:6060/action/init");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            print("Database reset");


            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
        yield return null;
    }

    IEnumerator GetInfo()
    {
        canUpdate = false;
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:6060/action/status");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            data = JsonUtility.FromJson<GatoData>(json);
            print(data);


            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
        yield return new WaitForSeconds(.3f);
        canUpdate = true;
        onGotData?.Invoke();
    }

    IEnumerator SendMove()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:6060/action/turn/" + id + "/" + box);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            print(www.downloadHandler.text);


            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator NewGame()
    {
        canUpdate = false;
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:6060/action/new_game");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            print("New Game");


            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
        yield return null;
    }
}
