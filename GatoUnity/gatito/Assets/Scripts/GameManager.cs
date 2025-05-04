using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Object references")]
    public GameObject playerSelection;
    public GameObject game;
    public Chat chat;
    public TextMeshProUGUI winner;

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
        if (idInput != "1" && idInput != "2")
        {
            Debug.LogWarning("Jugador inválido debe ser '1' o '2'.");
            return;
        }

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
        yield return new WaitForSeconds(3f);
        canUpdate = true;
    }

    public void PlayerMove(int boxInput)
    {
        if (data.board[boxInput] != 0) return; // ya ocupado
        chat.SendMove(boxInput);
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
