using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object references")]
    public GameObject playerSelection;
    public GameObject game;
    public Chat chat;

    [Header("Data Base")]
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

        chat.RequestGameStatus(); // Pedimos estado del juego al entrar
    }

    private void Update()
    {
        if (canUpdate && id != string.Empty)
        {
            chat.RequestGameStatus();
            canUpdate = false;
            StartCoroutine(EnableUpdateDelay());
        }
    }

    IEnumerator EnableUpdateDelay()
    {
        yield return new WaitForSeconds(0.3f);
        canUpdate = true;
    }

    public void PlayerMove(int boxInput)
    {
        box = boxInput;
        chat.SendMove(box);
    }

    [ContextMenu("newGame")]
    public void NewGameBtn()
    {
        chat.NewGame();
    }

    [ContextMenu("resetgame")]
    public void ResetGame()
    {
        chat.NewGame(); // Lo mismo por ahora
    }
}
