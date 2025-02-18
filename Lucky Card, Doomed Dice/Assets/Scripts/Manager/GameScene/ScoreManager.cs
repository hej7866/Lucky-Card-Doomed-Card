using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingleTon<ScoreManager>
{
    public int CalculateScore(int card, int dice)
    {
        return card * dice;
    }
}

