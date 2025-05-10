using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct GatoData
{
    public string userID;
    public int[] board;
    public int round;
    public int turn;
    public int score1;
    public int score2;
    public string player1;
    public string player2;
    public bool roundEnded;
    public string replay1;
    public string replay2;
}
