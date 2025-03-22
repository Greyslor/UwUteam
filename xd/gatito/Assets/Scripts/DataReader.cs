using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    public GameManager manager;
    public TextMeshProUGUI[] boardBoxes;
    public TextMeshProUGUI yourScore;
    public TextMeshProUGUI otherScore;
    public TextMeshProUGUI round;

    private void OnEnable()
    {
        manager.onGotData += UpdateBoard;
    }

    private void OnDisable()
    {
        manager.onGotData -= UpdateBoard;
    }

    public void UpdateBoard()
    {
        for (int i = 0; i < manager.data.board.Length; i++)
        {
            if (manager.data.board[i] == 1)
            {
                boardBoxes[i].text = "X";
            }
            else if (manager.data.board[i] == 2)
            {
                boardBoxes[i].text = "O";
            }
            else
            {
                boardBoxes[i].text = "";
            }
            Debug.Log(string.Join(",", manager.data.board));
        }
        UpdateScore();
    }

    public void UpdateScore()
    {
        if (manager.id == "&id=id1")
        {
            yourScore.text = "Your Score: " + manager.data.score1.ToString();
            otherScore.text = "Other Score: " + manager.data.score2.ToString();
        }
        else if (manager.id == "&id=id2")
        {
            yourScore.text = "Your Score: " + manager.data.score2.ToString();
            otherScore.text = "Other Score: " + manager.data.score1.ToString();
        }
        round.text = "Round: " + manager.data.round.ToString();
    }
}
